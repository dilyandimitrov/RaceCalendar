using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class DeleteAllRacesCommand : IDeleteAllRacesCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public DeleteAllRacesCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute()
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql);
    }

    private const string Sql = @"
DELETE FROM dbo.RaceInfo
DELETE FROM dbo.RaceDistances
DELETE FROM dbo.Races";
}
