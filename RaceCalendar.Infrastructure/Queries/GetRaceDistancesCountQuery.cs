using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Queries;

public class GetRaceDistancesCountQuery : IGetRaceDistancesCountQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetRaceDistancesCountQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IDictionary<int, int>> Get(ISet<int> raceIds)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var result = await conn.QueryAsync<RaceDistanceCountDto>(Sql,
            new
            {
                RaceIds = raceIds
            });

        return result.ToDictionary(x => x.RaceId, x => x.Count);
    }

    private const string Sql = $@"
SELECT 
    RaceId AS {nameof(RaceDistanceCountDto.RaceId)},
    COUNT(*) AS {nameof(RaceDistanceCountDto.Count)}
FROM dbo.RaceDistances
WHERE RaceId IN @RaceIds
GROUP BY RaceId
";

    private sealed class RaceDistanceCountDto
    {
        public int RaceId { get; set; } = default!;
        public int Count { get; set; } = default!;
    }
}
