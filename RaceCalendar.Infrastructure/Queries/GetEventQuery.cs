using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Queries;

public class GetEventQuery : IGetEventQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetEventQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<Event> QueryAsync(long id)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var result = await conn.QuerySingleOrDefaultAsync<Event>(Sql,
            new
            {
                Id = id
            });

        return result;
    }

    private const string Sql = $@"
SELECT *
FROM dbo.[Events]
WHERE Id = @Id
";
}
