using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Commands;

public interface ICreateEventCommand
{
    Task Execute(Event @event);
}
