using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface ICreateRaceRequestCommand
{
    Task Execute(RaceRequest raceRequest);
}
