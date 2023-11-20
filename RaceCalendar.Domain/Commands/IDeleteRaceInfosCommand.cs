namespace RaceCalendar.Domain.Commands;

public interface IDeleteRaceInfosCommand
{
    Task Execute(ISet<int> ids);
}
