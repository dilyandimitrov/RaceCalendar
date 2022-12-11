using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateRaceInfoCommand
{
    Task Execute(RaceInfo raceInfo);
}
