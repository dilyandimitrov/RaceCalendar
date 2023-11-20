using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IGeoDataService
{
    Task<IEnumerable<GeoPoint>> Search(GetRacesFilterInput filter);
}
