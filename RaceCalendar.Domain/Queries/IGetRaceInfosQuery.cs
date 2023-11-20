using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetRaceInfosQuery
{
    Task<IDictionary<int, IEnumerable<RaceInfo>>> Get(int raceId, ISet<int> raceDistanceIds);
}
