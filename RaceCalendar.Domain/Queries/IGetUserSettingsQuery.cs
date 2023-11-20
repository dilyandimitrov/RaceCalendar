using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetUserSettingsQuery
{
    Task<UserSettings> Get(int id);
    Task<UserSettings?> Get(string userId);
}
