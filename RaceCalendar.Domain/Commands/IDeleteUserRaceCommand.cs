using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IDeleteUserRaceCommand
{
    Task Execute(int id);
}
