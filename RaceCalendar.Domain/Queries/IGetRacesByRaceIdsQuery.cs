using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetRacesByRaceIdsQuery
{
    Task<IReadOnlyDictionary<int, Race>> Get(ISet<int> raceIds);
}
