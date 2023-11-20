using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface ICreateRaceInfoCommand
{
    Task Execute(RaceInfo raceInfo);
}
