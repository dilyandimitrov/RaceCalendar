using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetGeoDataQuery
{
    Task<IEnumerable<RaceGeoData>> Get(GetRacesFilterInput filter);
}
