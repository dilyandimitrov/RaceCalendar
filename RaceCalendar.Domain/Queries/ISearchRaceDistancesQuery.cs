using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface ISearchRaceDistancesQuery
{
    Task<IEnumerable<RaceDistance>> Get(GetRacesFilterInput filter, ISet<int> raceIds);
}
