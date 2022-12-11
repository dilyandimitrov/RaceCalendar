using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetRaceDistancesQuery
{
    Task<IEnumerable<RaceDistance>> Get(int raceId, ISet<int>? raceDistanceIds = null);
}
