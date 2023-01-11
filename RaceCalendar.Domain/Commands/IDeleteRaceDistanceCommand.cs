using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IDeleteRaceDistanceCommand
{
    Task Execute(ISet<int> ids);
}
