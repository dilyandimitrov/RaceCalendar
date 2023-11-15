using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Queries;

public interface IGetAllUpcomingEventsQuery
{
    Task<IEnumerable<Event>> QueryAsync(bool showPublicOnly = false);
}
