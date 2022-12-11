using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetUserSettingsQuery
{
    Task<UserSettings> Get(int id);
    Task<UserSettings?> Get(string userId);
}
