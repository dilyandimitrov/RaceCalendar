using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Queries;

public interface IGetEventQuery
{
    Task<Event> QueryAsync(long id);
}
