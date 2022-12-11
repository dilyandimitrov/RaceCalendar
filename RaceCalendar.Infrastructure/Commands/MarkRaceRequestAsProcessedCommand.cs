using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class MarkRaceRequestAsProcessedCommand : IMarkRaceRequestAsProcessedCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public MarkRaceRequestAsProcessedCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(int raceRequestId)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = raceRequestId
        });
    }

    private const string Sql = $@"
UPDATE dbo.RaceRequests
SET IsProcessed = 1
WHERE Id = @Id
";
}
