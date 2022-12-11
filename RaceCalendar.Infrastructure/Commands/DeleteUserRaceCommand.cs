using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class DeleteUserRaceCommand : IDeleteUserRaceCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public DeleteUserRaceCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(int id)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = id
        });
    }

    private const string Sql = @"
DELETE dbo.UserRaces
WHERE Id = @Id";
}
