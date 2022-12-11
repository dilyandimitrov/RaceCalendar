using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateUserRaceCommand
{
    Task Execute(UserRace userRace);
}
