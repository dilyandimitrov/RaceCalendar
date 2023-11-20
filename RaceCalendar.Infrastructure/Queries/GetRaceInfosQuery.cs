using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Queries;

public class GetRaceInfosQuery : IGetRaceInfosQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetRaceInfosQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IDictionary<int, IEnumerable<RaceInfo>>> Get(int raceId, ISet<int> raceDistanceIds)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var raceInfos = (await conn.QueryAsync<RaceInfoDto>(Sql, new
        {
            RaceId = raceId,
            RaceDistanceIds = raceDistanceIds
        })).ToList();

        return raceInfos
            .GroupBy(x => x.RaceDistanceId)
            .ToDictionary(
                x => x.Key,
                x => x.Select(x => new RaceInfo(x.Id, x.RaceId, x.RaceDistanceId, x.Name, x.Value)));
    }

    private sealed class RaceInfoDto
    {
        public int Id { get; set; } = default!;
        public int RaceId { get; set; } = default!;
        public int RaceDistanceId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Value { get; set; } = default!;
    }

    private const string Sql = @$"
SELECT [Id] AS {nameof(RaceInfoDto.Id)}
      ,[RaceId] AS {nameof(RaceInfoDto.RaceId)}
      ,[RaceDistanceId] AS {nameof(RaceInfoDto.RaceDistanceId)}
      ,[Name] AS {nameof(RaceInfoDto.Name)}
      ,[Value] AS {nameof(RaceInfoDto.Value)}
FROM [dbo].[RaceInfo]
WHERE RaceId = @RaceId AND RaceDistanceId IN @RaceDistanceIds
";
}
