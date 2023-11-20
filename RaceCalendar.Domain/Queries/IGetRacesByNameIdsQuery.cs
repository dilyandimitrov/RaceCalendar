using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetRacesByNameIdsQuery
{
    Task<IReadOnlyDictionary<string, Race>> Get(ISet<string> nameIds);
}
