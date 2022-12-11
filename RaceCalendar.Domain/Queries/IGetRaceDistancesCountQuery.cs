using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetRaceDistancesCountQuery
{
    Task<IDictionary<int, int>> Get(ISet<int> raceIds);
}
