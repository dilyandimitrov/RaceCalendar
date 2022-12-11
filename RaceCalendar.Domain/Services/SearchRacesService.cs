using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Models.Paging;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services;

public class SearchRacesService : ISearchRacesService
{
    private readonly ISearchRacesValidatorService _searchRacesValidatorService;
    private readonly ISearchRacesQuery _getRacesQuery;
    private readonly ISearchRaceDistancesQuery _getRaceDistancesQuery;
    private readonly IGetRaceDistancesCountQuery _getRaceDistancesCountQuery;

    public SearchRacesService(
        ISearchRacesValidatorService searchRacesValidatorService,
        ISearchRacesQuery getRacesQuery,
        ISearchRaceDistancesQuery getRaceDistancesQuery,
        IGetRaceDistancesCountQuery getRaceDistancesCountQuery)
    {
        _searchRacesValidatorService = searchRacesValidatorService ?? throw new ArgumentNullException(nameof(searchRacesValidatorService));
        _getRacesQuery = getRacesQuery ?? throw new ArgumentNullException(nameof(getRacesQuery));
        _getRaceDistancesQuery = getRaceDistancesQuery ?? throw new ArgumentNullException(nameof(getRaceDistancesQuery));
        _getRaceDistancesCountQuery = getRaceDistancesCountQuery ?? throw new ArgumentNullException(nameof(getRaceDistancesCountQuery));
    }

    public async Task<PagedResult<Race>> Search(GetRacesFilterInput filter, int page, int pageSize)
    {
        _searchRacesValidatorService.ValidateSearch(filter);

        var (races, totalRaces) = await _getRacesQuery.Get(filter, page, pageSize);

        var raceIds = races.Select(r => r.Id).ToHashSet();

        var raceDistances = await _getRaceDistancesQuery.Get(filter, raceIds);
        var distanceCountsMap = await _getRaceDistancesCountQuery.Get(raceIds);

        races = races.ToList();

        foreach (var race in races)
        {
            var distances = raceDistances.Where(x => x.RaceId == race.Id).ToList();

            if (distances != null && distances.Count > 0)
            {
                race.Distances = distances;
            }

            race.AllDistances = 
                distanceCountsMap.TryGetValue(race.Id, out var distancesCount) && 
                race.Distances.Count() == distancesCount;
        }

        var result = new PagedResult<Race>(page, pageSize, totalRaces, races);

        return result;
    }
}
