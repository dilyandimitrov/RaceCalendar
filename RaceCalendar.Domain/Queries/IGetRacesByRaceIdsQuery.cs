using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetRacesByRaceIdsQuery
{
    Task<IReadOnlyDictionary<int, Race>> Get(ISet<int> raceIds);
}
