using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetRaceInfosQuery
{
    Task<IDictionary<int, IEnumerable<RaceInfo>>> Get(int raceId, ISet<int> raceDistanceIds);
}
