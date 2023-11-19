namespace RaceCalendar.Domain.Commands;

public interface IDeleteEventCommentCommand
{
    Task Execute(long id);
}
