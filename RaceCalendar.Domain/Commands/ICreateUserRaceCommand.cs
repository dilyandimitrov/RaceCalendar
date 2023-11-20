using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface ICreateUserRaceCommand
{
    Task Execute(UserRace userRace);
}
