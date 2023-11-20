using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface ISearchRacesQuery
{
    Task<(IEnumerable<Race>, int total)> Get(GetRacesFilterInput filter, int page, int pageSize);
}
