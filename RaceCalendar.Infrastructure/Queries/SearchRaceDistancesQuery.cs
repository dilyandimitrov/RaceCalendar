using Dapper;
using Microsoft.Data.SqlClient;
using NodaTime;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Queries;

public class SearchRaceDistancesQuery : ISearchRaceDistancesQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public SearchRaceDistancesQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<RaceDistance>> Get(GetRacesFilterInput filter, ISet<int> raceIds)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var distances = (await conn.QueryAsync<RaceDistanceDto>(SearchRaceDistancesSql, new
        {
            ShowPrevious = filter.ShowPrevious ?? 0,
            StartDate = filter.FromDate?.ToDateTimeUnspecified(),
            EndDate = filter.ToDate?.ToDateTimeUnspecified(),
            FromDistance = filter.FromDistance,
            ToDistance = filter.ToDistance,
            Text = $"%{filter.Text}%",
            Terrain = filter.Terrain,
            Special = filter.Special,
            RaceIds = raceIds
        })).ToList();

        return distances.Select(d =>
            new RaceDistance(
                d.Id,
                d.RaceId,
                d.Name,
                d.Distance, 
                d.StartDate, 
                d.StartTime, 
                d.UnconfirmedDate, 
                d.ElevationGain, 
                d.Price, 
                d.Link, 
                d.ResultsLink,
                d.Latitude,
                d.Longitude));
    }

    private sealed class RaceDistanceDto
    {
        public int Id { get; set; } = default;
        public int RaceId { get; set; } = default;
        public string Name { get; init; } = default!;
        public double? Distance { get; init; } = default!;
        public LocalDate? StartDate { get; init; } = default!;
        public TimeSpan? StartTime { get; init; } = default!;
        public int? UnconfirmedDate { get; init; } = default!;
        public int? ElevationGain { get; init; } = default!;
        public string Price { get; init; } = default!;
        public string Link { get; init; } = default!;
        public string ResultsLink { get; init; } = default!;
        public decimal? Latitude { get; init; } = default!;
        public decimal? Longitude { get; init; } = default!;
    }

    private const string SearchRaceDistancesSql = $@"
WITH Data_CTE
AS
(
    SELECT DISTINCT RD.*
    FROM 
        dbo.Races R INNER JOIN dbo.RaceDistances RD ON RD.RaceId = R.Id
    WHERE
        RD.StartDate < @StartDate AND
        {FilterWithoutDatesSql}
    UNION
    SELECT DISTINCT RD.*
    FROM 
        dbo.Races R INNER JOIN dbo.RaceDistances RD ON RD.RaceId = R.Id
    WHERE
        {FullFilterSql}
)
SELECT * FROM Data_CTE RD
WHERE RD.RaceId in @RaceIds
";

    private const string FilterWithoutDatesSql = $@"
(@FromDistance IS NULL OR RD.Distance >= @FromDistance) AND 
(@ToDistance IS NULL OR RD.Distance <= @ToDistance OR RD.Distance IS NULL) AND
(R.Name LIKE @Text OR R.NameId LIKE @Text OR R.Tags LIKE @Text OR R.Link LIKE @Text OR RD.Name LIKE @Text) AND
(@Terrain IS NULL OR (R.Terrain & @Terrain) > 0) AND
    ((@Special = 32 AND 1 = 1) OR 
    (@Special IS NULL AND ((R.Special ^ 32) > 32 OR R.Special IS NULL)) OR
    (@Special IS NOT NULL AND @Special <> 32 AND ((R.Special & @Special) > 0)))
";

    private const string FullFilterSql = $@"
{DateFilterSql} AND
(@FromDistance IS NULL OR RD.Distance >= @FromDistance) AND 
(@ToDistance IS NULL OR RD.Distance <= @ToDistance OR RD.Distance IS NULL) AND
(R.Name LIKE @Text OR R.NameId LIKE @Text OR R.Tags LIKE @Text OR R.Link LIKE @Text OR RD.Name LIKE @Text) AND
(@Terrain IS NULL OR (R.Terrain & @Terrain) > 0) AND
    ((@Special = 32 AND 1 = 1) OR 
    (@Special IS NULL AND ((R.Special ^ 32) > 32 OR R.Special IS NULL)) OR
    (@Special IS NOT NULL AND @Special <> 32 AND ((R.Special & @Special) > 0)))
";

    private const string DateFilterSql = $@"
(@StartDate IS NULL OR RD.StartDate >= @StartDate) AND
(@EndDate IS NULL OR RD.StartDate <= @EndDate)
";
}
