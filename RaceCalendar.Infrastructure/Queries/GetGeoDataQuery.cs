using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Queries;

public class GetGeoDataQuery : IGetGeoDataQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetGeoDataQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<RaceGeoData>> Get(GetRacesFilterInput filter)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var races = (await conn.QueryAsync<RaceGeoDataDto>(SearchRacesSql, new
        {
            ShowPrevious = filter.ShowPrevious ?? 0,
            StartDate = filter.FromDate?.ToDateTimeUnspecified(),
            EndDate = filter.ToDate?.ToDateTimeUnspecified(),
            FromDistance = filter.FromDistance,
            ToDistance = filter.ToDistance,
            Text = $"%{filter.Text}%",
            Terrain = filter.Terrain,
            Special = filter.Special
        })).ToList();

        if (!races.Any())
        {
            return Enumerable.Empty<RaceGeoData>();
        }

        return 
            races.Select(r =>
                new RaceGeoData(
                    r.Id,
                    r.NameId,
                    r.Name,
                    r.Latitude,
                    r.Longitude,
                    r.Special,
                    r.StartDate));
    }

    private sealed class RaceGeoDataDto
    {
        public int Id { get; set; } = default;
        public string Name { get; init; } = default!;
        public string NameId { get; set; } = default!;
        public decimal Latitude { get; set; } = default!;
        public decimal Longitude { get; set; } = default!;
        public Specials Special { get; set; } = default!;
        public DateTime StartDate { get; set; } = default!;
    }

    private const string SearchRacesSql = $@"
WITH Data_CTE
AS
(
    {GetPreviousRacesSql}
    UNION
    SELECT DISTINCT
        R.Id AS [{nameof(RaceGeoDataDto.Id)}],
        R.Name AS [{nameof(RaceGeoDataDto.Name)}],
        R.NameId AS [{nameof(RaceGeoDataDto.NameId)}],
        R.Latitude AS [{nameof(RaceGeoDataDto.Latitude)}],
        R.Longitude AS [{nameof(RaceGeoDataDto.Longitude)}],
        R.Special AS [{nameof(RaceGeoDataDto.Special)}],
        R.StartDate AS [{nameof(RaceGeoDataDto.StartDate)}]
    FROM 
        dbo.Races R INNER JOIN dbo.RaceDistances RD ON RD.RaceId = R.Id
    WHERE
        {FullFilterSql}
)
SELECT * FROM Data_CTE";

    private const string GetPreviousRacesSql = $@"
SELECT DISTINCT TOP (@ShowPrevious) 
    R.Id AS [{nameof(RaceGeoDataDto.Id)}],
    R.Name AS [{nameof(RaceGeoDataDto.Name)}],
    R.NameId AS [{nameof(RaceGeoDataDto.NameId)}],
    R.Latitude AS [{nameof(RaceGeoDataDto.Latitude)}],
    R.Longitude AS [{nameof(RaceGeoDataDto.Longitude)}],
    R.Special AS [{nameof(RaceGeoDataDto.Special)}],
    R.StartDate AS [{nameof(RaceGeoDataDto.StartDate)}]
FROM 
    dbo.Races R INNER JOIN dbo.RaceDistances RD ON RD.RaceId = R.Id
WHERE
    RD.StartDate < @StartDate AND
    {FilterWithoutDatesSql}";

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
}
