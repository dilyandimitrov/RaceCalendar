using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateRaceCommand
{
    Task Execute(Race race);
}
