using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class UpdateEventVisitorsCountCommand : IUpdateEventVisitorsCountCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public UpdateEventVisitorsCountCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(long eventId, long visitorsCount)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = eventId,
            VisitorsCount = visitorsCount
        });
    }

    private const string Sql = @"
UPDATE [dbo].[Events]
SET [VisitorsCount] = @VisitorsCount
WHERE Id = @Id";
}
