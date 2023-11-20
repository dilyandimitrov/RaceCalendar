using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetUserRacesByUserQuery
{
    Task<IEnumerable<UserRace>> Get(string userId);
}
