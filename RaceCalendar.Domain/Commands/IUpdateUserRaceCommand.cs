using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateUserRaceCommand
{
    Task Execute(UserRace userRace);
}
