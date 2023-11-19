using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateEventCommentCommand
{
    Task Execute(EventComment eventComment);
}
