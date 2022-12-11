using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IUserSettingsService
{
    Task<UserSettings> Get(string userId);
    Task Update(UserSettingsForUpdate userSettings);
}
