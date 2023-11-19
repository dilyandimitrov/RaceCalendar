using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IEventCommentService
{
    Task Create(EventComment eventComment);
    Task Update(EventComment eventComment);
    Task Delete(long id, string userId);
    Task<IEnumerable<EventComment>> GetCommentsByEvent(long eventId);
}
