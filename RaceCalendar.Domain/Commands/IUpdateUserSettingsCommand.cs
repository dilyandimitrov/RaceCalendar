using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateUserSettingsCommand
{
    Task Execute(UserSettingsForUpdate userSettings);
}
