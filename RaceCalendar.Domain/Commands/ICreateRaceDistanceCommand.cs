using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface ICreateRaceDistanceCommand
{
    Task Execute(RaceDistance raceDistance);
}
