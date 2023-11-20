using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateRaceCommand
{
    Task Execute(Race race);
}
