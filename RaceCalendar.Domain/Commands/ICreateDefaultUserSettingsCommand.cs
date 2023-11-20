using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface ICreateDefaultUserSettingsCommand
{
    Task<UserSettings> Execute(string userId);
}
