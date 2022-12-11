using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetRacesByNameIdsQuery
{
    Task<IReadOnlyDictionary<string, Race>> Get(ISet<string> nameIds);
}
