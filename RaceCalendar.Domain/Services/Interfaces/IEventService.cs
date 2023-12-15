using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IEventService
{
    Task Create(Event @event);
    Task Update(Event @event);
    Task<Event?> Get(long id);
    Task Delete(long id, string userId);
}
