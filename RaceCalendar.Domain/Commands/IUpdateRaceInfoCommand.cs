using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateRaceInfoCommand
{
    Task Execute(RaceInfo raceInfo);
}
