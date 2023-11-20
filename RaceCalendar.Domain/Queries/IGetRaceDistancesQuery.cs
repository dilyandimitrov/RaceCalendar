using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetRaceDistancesQuery
{
    Task<IEnumerable<RaceDistance>> Get(int raceId, ISet<int>? raceDistanceIds = null);
}
