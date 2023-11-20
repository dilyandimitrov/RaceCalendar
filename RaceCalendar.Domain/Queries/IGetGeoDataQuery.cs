using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetGeoDataQuery
{
    Task<IEnumerable<RaceGeoData>> Get(GetRacesFilterInput filter);
}
