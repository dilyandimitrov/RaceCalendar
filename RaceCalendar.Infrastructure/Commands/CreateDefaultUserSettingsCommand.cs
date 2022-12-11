using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateDefaultUserSettingsCommand : ICreateDefaultUserSettingsCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateDefaultUserSettingsCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<UserSettings> Execute(string userId)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var id = await conn.ExecuteAsync(Sql, new
        {
            UserId = userId,
            CreatedOn = DateTime.UtcNow,
            RacesFilterYear = UserSettingsYears.Upcoming,
            RacesShowPassed = false
        });

        var userSettings = new UserSettings(
            id,
            userId,
            DateTime.UtcNow,
            null,
            UserSettingsYears.Upcoming,
            false);

        return userSettings;
    }

    private const string Sql = $@"
INSERT INTO [dbo].[UserSettings]
    ([UserId]
    ,[CreatedOn]
    ,[RacesFilterYear]
    ,[RacesShowPassed])
VALUES
    (@UserId
    ,@CreatedOn
    ,@RacesFilterYear
    ,@RacesShowPassed)
";
}
