using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface ISearchEventsService
{
    Task<IEnumerable<Event>> GetUpcomingEvents(bool showPublicOnly = false);
}
