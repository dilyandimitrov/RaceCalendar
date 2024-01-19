using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IEventNotificationService
{
    Task NotifyNewEvent(Event @event);
}
