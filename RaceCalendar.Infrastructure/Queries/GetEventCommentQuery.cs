using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Queries;

public class GetEventCommentQuery : IGetEventCommentQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetEventCommentQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<EventComment> QueryAsync(long id)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var result = await conn.QuerySingleOrDefaultAsync<EventComment>(Sql,
            new
            {
                Id = id
            });

        return result;
    }

    private const string Sql = $@"
SELECT *
FROM dbo.[EventComments]
WHERE Id = @Id";
}