using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IDeleteRaceCommand
{
    Task Execute(int raceId);
}
