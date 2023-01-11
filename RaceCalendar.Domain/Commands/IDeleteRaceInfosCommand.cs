using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IDeleteRaceInfosCommand
{
    Task Execute(ISet<int> ids);
}
