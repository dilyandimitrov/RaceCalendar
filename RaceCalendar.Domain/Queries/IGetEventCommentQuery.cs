using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Queries;

public interface IGetEventCommentQuery
{
    Task<EventComment> QueryAsync(long id);
}
