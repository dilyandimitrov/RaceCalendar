using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IGetUserSettingsQuery _getUserSettingsQuery;
    private readonly ICreateDefaultUserSettingsCommand _createDefaultUserSettingsCommand;
    private readonly IUpdateUserSettingsCommand _updateUserSettingsCommand;

    public UserSettingsService(
        IGetUserSettingsQuery getUserSettingsQuery,
        ICreateDefaultUserSettingsCommand createDefaultUserSettingsCommand,
        IUpdateUserSettingsCommand updateUserSettingsCommand)
    {
        _getUserSettingsQuery = getUserSettingsQuery ?? throw new ArgumentNullException(nameof(getUserSettingsQuery));
        _createDefaultUserSettingsCommand = createDefaultUserSettingsCommand ?? throw new ArgumentNullException(nameof(createDefaultUserSettingsCommand));
        _updateUserSettingsCommand = updateUserSettingsCommand ?? throw new ArgumentNullException(nameof(updateUserSettingsCommand));
    }

    public async Task<UserSettings> Get(string userId)
    {
        var userSettings = await _getUserSettingsQuery.Get(userId);

        if (userSettings is null)
        {
            userSettings = await _createDefaultUserSettingsCommand.Execute(userId);
        }

        return userSettings;
    }

    public async Task Update(UserSettingsForUpdate userSettings)
    {
        var settings = await _getUserSettingsQuery.Get(userSettings.Id);

        if (settings is null)
        {
            return;
        }

        await _updateUserSettingsCommand.Execute(userSettings);
    }
}
