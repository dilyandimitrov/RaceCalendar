using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Commands;

public interface ICreateEventCommentCommand
{
    Task Execute(EventComment eventComment);
}
