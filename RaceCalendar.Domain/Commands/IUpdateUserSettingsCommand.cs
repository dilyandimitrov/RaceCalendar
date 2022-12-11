using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateUserSettingsCommand
{
    Task Execute(UserSettingsForUpdate userSettings);
}
