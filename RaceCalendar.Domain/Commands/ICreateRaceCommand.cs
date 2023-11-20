using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface ICreateRaceCommand
{
    Task Execute(Race race);
}
