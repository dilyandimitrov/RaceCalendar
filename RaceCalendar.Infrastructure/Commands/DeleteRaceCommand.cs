using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class DeleteRaceCommand : IDeleteRaceCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public DeleteRaceCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(int raceId)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            RaceId = raceId
        });
    }

    private const string Sql = @"
DELETE dbo.Races
WHERE Id = @RaceId";
}
