using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetSystemInfoQuery
{
    Task<IEnumerable<SystemInfo>> Get();
}
