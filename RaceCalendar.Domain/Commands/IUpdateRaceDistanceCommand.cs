using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateRaceDistanceCommand
{
    Task Execute(RaceDistance raceDistance);
}
