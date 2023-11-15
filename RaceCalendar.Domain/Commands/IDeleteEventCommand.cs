using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IDeleteEventCommand
{
    Task Execute(long id);
}
