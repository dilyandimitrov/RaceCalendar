using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Queries;

public interface IGetEventCommentsQuery
{
    Task<IEnumerable<EventComment>> QueryAsync(long eventId);
}
