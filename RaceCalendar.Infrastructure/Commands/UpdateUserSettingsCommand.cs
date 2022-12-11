using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class UpdateUserSettingsCommand : IUpdateUserSettingsCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public UpdateUserSettingsCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(UserSettingsForUpdate userSettings)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = userSettings.Id,
            RacesFilterYear = userSettings.RacesFilterYear,
            RacesShowPassed = userSettings.RacesShowPassed,
            UpdatedOn = DateTime.UtcNow
        });
    }

    private const string Sql = $@"
UPDATE dbo.UserSettings
SET RacesFilterYear = @RacesFilterYear,
    RacesShowPassed = @RacesShowPassed,
    UpdatedOn = @UpdatedOn
WHERE Id = @Id
";
}
