using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Queries;

public class GetAllUpcomingEventsQuery : IGetAllUpcomingEventsQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetAllUpcomingEventsQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<Event>> QueryAsync(bool showPublicOnly = false)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var result = await conn.QueryAsync<Event>(Sql,
            new
            {
                Now = DateTime.UtcNow,
                ShowPublicOnly = showPublicOnly
            });

        return result.ToList();
    }

    private const string Sql = $@"
SELECT *
FROM dbo.[Events]
WHERE StartDate > @Now AND
(@ShowPublicOnly = 0 OR IsPublic = @ShowPublicOnly)
";
}
