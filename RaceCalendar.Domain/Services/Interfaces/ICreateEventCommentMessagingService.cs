using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface ICreateEventCommentMessagingService
{
    Task SendMessage(EventComment eventComment);
}
