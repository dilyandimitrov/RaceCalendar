using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IGeoDataService
{
    Task<IEnumerable<GeoPoint>> Search(GetRacesFilterInput filter);
}
