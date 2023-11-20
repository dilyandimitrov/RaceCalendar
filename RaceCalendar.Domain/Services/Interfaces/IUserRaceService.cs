using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IUserRaceService
{
    Task Create(UserRace userRace);
    Task<IEnumerable<UserRace>> Get(string userId);
    Task<IEnumerable<UserRace>> GetAllByUser(string userId);
    Task Update(UserRace userRace);
    Task Delete(int id);
}
