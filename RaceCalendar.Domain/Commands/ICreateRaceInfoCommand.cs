using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface ICreateRaceInfoCommand
{
    Task Execute(RaceInfo raceInfo);
}
