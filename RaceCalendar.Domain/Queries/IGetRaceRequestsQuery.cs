using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetRaceRequestsQuery
{
    Task<IEnumerable<RaceRequest>> Get(bool includeProcessed = false);
}
