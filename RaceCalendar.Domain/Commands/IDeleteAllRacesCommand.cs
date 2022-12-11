using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IDeleteAllRacesCommand
{
    Task Execute();
}
