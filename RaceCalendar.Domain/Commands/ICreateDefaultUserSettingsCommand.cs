using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface ICreateDefaultUserSettingsCommand
{
    Task<UserSettings> Execute(string userId);
}
