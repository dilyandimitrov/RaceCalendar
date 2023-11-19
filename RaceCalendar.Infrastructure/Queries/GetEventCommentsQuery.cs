using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Queries;

public class GetEventCommentsQuery : IGetEventCommentsQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetEventCommentsQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<EventComment>> QueryAsync(long eventId)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var result = await conn.QueryAsync<EventComment>(Sql,
            new
            {
                EventId = eventId
            });

        return result;
    }

    private const string Sql = $@"
SELECT *
FROM dbo.[EventComments]
WHERE EventId = @EventId";
}
