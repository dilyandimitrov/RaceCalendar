using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Models.Paging;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface ISearchRacesService
{
    Task<PagedResult<Race>> Search(GetRacesFilterInput filter, int page, int pageSize);
}
