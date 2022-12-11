using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface ISearchRacesQuery
{
    Task<(IEnumerable<Race>, int total)> Get(GetRacesFilterInput filter, int page, int pageSize);
}
