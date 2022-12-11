using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetRaceRequestsQuery
{
    Task<IEnumerable<RaceRequest>> Get(bool includeProcessed = false);
}
