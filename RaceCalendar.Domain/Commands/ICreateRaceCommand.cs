using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface ICreateRaceCommand
{
    Task Execute(Race race);
}
