namespace RaceCalendar.Domain.Commands;

public interface IDeleteRaceDistanceCommand
{
    Task Execute(ISet<int> ids);
}
