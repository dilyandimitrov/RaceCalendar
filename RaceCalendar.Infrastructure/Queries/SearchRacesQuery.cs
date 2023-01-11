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

public class SearchRacesQuery : ISearchRacesQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public SearchRacesQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<(IEnumerable<Race>, int)> Get(GetRacesFilterInput filter, int page, int pageSize)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var races = (await conn.QueryAsync<RaceDto>(SearchRacesSql, new
        {
            ShowPrevious = filter.ShowPrevious ?? 0,
            Page = (page - 1) * pageSize,
            PageSize = pageSize,
            StartDate = filter.FromDate?.ToDateTimeUnspecified(),
            EndDate = filter.ToDate?.ToDateTimeUnspecified(),
            FromDistance = filter.FromDistance,
            ToDistance = filter.ToDistance,
            Text = $"%{filter.Text}%",
            Terrain = filter.Terrain,
            Special = filter.Special,
        })).ToList();

        if (!races.Any())
        {
            return (Enumerable.Empty<Race>(), default(int));
        }

        return (
            races.Select(r =>
                new Race(
                    r.Id,
                    r.Name,
                    r.NameId,
                    r.Country,
                    r.City,
                    r.StartDate,
                    r.EndDate,
                    r.Link,
                    r.Tags,
                    r.Cancelled,
                    r.Terrain,
                    r.Special)),
            races.First().TotalRows);
    }

    private sealed class RaceDto
    {
        public int Id { get; set; } = default;
        public string Name { get; init; } = default!;
        public string NameId { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string City { get; set; } = default!;
        public LocalDate? StartDate { get; set; } = default!;
        public LocalDate? EndDate { get; set; } = default!;
        public string Link { get; set; } = default!;
        public string Tags { get; set; } = default!;
        public Cancelled? Cancelled { get; set; } = null;
        public Terrains? Terrain { get; set; } = null;
        public Specials? Special { get; set; } = null;
        public int TotalRows { get; set; } = default;
    }

    private const string SearchRacesSql = $@"
WITH Data_CTE
AS
(
    {GetPreviousRacesSql}
    UNION
    SELECT DISTINCT
        R.Id AS [{nameof(RaceDto.Id)}],
        R.Name AS [{nameof(RaceDto.Name)}],
        R.NameId AS [{nameof(RaceDto.NameId)}],
        R.Country AS [{nameof(RaceDto.Country)}],
        R.City AS [{nameof(RaceDto.City)}],
        R.StartDate AS [{nameof(RaceDto.StartDate)}],
        R.EndDate AS [{nameof(RaceDto.EndDate)}],
        R.Link AS [{nameof(RaceDto.Link)}],
        R.Tags AS [{nameof(RaceDto.Tags)}],
        R.Terrain AS [{nameof(RaceDto.Terrain)}],
        R.Special AS [{nameof(RaceDto.Special)}],
        R.Cancelled AS [{nameof(RaceDto.Cancelled)}]
    FROM 
        dbo.Races R INNER JOIN dbo.RaceDistances RD ON RD.RaceId = R.Id
    WHERE
        {FullFilterSql}
),
Count_CTE
AS 
(
    SELECT COUNT(*) AS TotalRows FROM Data_CTE
)
SELECT * FROM Data_CTE CROSS JOIN Count_CTE
ORDER BY StartDate, Name
OFFSET @Page ROWS
FETCH NEXT @PageSize ROWS ONLY
";

    private const string GetPreviousRacesSql = $@"
SELECT DISTINCT TOP (@ShowPrevious) 
    R.Id AS [{nameof(RaceDto.Id)}],
    R.Name AS [{nameof(RaceDto.Name)}],
    R.NameId AS [{nameof(RaceDto.NameId)}],
    R.Country AS [{nameof(RaceDto.Country)}],
    R.City AS [{nameof(RaceDto.City)}],
    R.StartDate AS [{nameof(RaceDto.StartDate)}],
    R.EndDate AS [{nameof(RaceDto.EndDate)}],
    R.Link AS [{nameof(RaceDto.Link)}],
    R.Tags AS [{nameof(RaceDto.Tags)}],
    R.Terrain AS [{nameof(RaceDto.Terrain)}],
    R.Special AS [{nameof(RaceDto.Special)}],
    R.Cancelled AS [{nameof(RaceDto.Cancelled)}]
FROM 
    dbo.Races R INNER JOIN dbo.RaceDistances RD ON RD.RaceId = R.Id
WHERE
    RD.StartDate < @StartDate AND
    {FilterWithoutDatesSql}
ORDER BY R.StartDate DESC";

    private const string FilterWithoutDatesSql = $@"
(@FromDistance IS NULL OR RD.Distance >= @FromDistance) AND 
(@ToDistance IS NULL OR RD.Distance <= @ToDistance OR RD.Distance IS NULL) AND
(R.Name LIKE @Text OR R.NameId LIKE @Text OR R.Tags LIKE @Text OR R.Link LIKE @Text OR RD.Name LIKE @Text) AND
(@Terrain IS NULL OR (R.Terrain & @Terrain) > 0) AND
    ((@Special = 32 AND 1 = 1) OR 
    (@Special IS NULL AND ((R.Special ^ 32) > 32 OR R.Special IS NULL)) OR
    (@Special IS NOT NULL AND @Special <> 32 AND ((R.Special & @Special) > 0)))
";

    private const string FullFilterSql = $@"{DateFilterSql} AND {FilterWithoutDatesSql}";

    private const string DateFilterSql = $@"
(@StartDate IS NULL OR RD.StartDate >= @StartDate) AND
(@EndDate IS NULL OR RD.StartDate <= @EndDate)
";

    private const string SqlRaceDistanceCount = $@"
SELECT COUNT(*)
FROM dbo.RaceDistances
WHERE RaceId = @RaceId
";
}
