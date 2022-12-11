using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateRaceDistanceCommand
{
    Task Execute(RaceDistance raceDistance);
}
