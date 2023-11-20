using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class DeleteUserSettingsCommand : IDeleteUserSettingsCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public DeleteUserSettingsCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(string userId)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            UserId = userId
        });
    }

    private const string Sql = @"
DELETE dbo.UserSettings
WHERE UserId = @UserId";
}
