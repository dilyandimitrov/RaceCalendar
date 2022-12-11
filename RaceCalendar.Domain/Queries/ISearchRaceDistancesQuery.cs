using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface ISearchRaceDistancesQuery
{
    Task<IEnumerable<RaceDistance>> Get(GetRacesFilterInput filter, ISet<int> raceIds);
}
