using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface ICreateRaceRequestCommand
{
    Task Execute(RaceRequest raceRequest);
}
