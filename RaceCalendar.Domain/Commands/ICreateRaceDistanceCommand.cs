using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface ICreateRaceDistanceCommand
{
    Task Execute(RaceDistance raceDistance);
}
