using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface ICreateUserRaceCommand
{
    Task Execute(UserRace userRace);
}
