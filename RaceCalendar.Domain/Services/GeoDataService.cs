using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Domain.Services;

public class GeoDataService : IGeoDataService
{
    private readonly ISearchRacesValidatorService _searchRacesValidatorService;
    private readonly IGetGeoDataQuery _getGeoDataQuery;
    private readonly ISearchRaceDistancesQuery _getRaceDistancesQuery;
    private readonly List<decimal> _emptyPoint = new() { 0, 0 };

    public GeoDataService(
        ISearchRacesValidatorService searchRacesValidatorService,
        IGetGeoDataQuery getGeoDataQuery,
        ISearchRaceDistancesQuery getRaceDistancesQuery)
    {
        _searchRacesValidatorService = searchRacesValidatorService ?? throw new ArgumentNullException(nameof(searchRacesValidatorService));
        _getGeoDataQuery = getGeoDataQuery ?? throw new ArgumentNullException(nameof(getGeoDataQuery));
        _getRaceDistancesQuery = getRaceDistancesQuery ?? throw new ArgumentNullException(nameof(getRaceDistancesQuery));
    }

    public async Task<IEnumerable<GeoPoint>> Search(GetRacesFilterInput filter)
    {
        _searchRacesValidatorService.ValidateSearch(filter);

        var geoData = await _getGeoDataQuery.Get(filter);

        if (geoData is null || !geoData.Any())
        {
            return Enumerable.Empty<GeoPoint>();
        }    

        var raceIds = geoData.Select(r => r.Id).ToHashSet();
        var raceDistances = await _getRaceDistancesQuery.Get(filter, raceIds);
        var raceDistancesWithGeoData = raceDistances.Where(r => r.Longitude is not null && r.Latitude is not null).ToList();
        
        var racesPoints = geoData.Select(g =>
        {
            var raceDistancesWithoutGeoData = raceDistances.Where(rd => rd.RaceId == g.Id && (rd.Longitude is null || rd.Latitude is null));
            
            return new GeoPoint("Feature",
                new Geometry("Point", new List<decimal>() { g.Latitude, g.Longitude }),
                new Properties(g.Name, g.NameId, g.StartDate,
                    raceDistancesWithoutGeoData
                        .Select(rd => new RaceDistanceSimple(rd.Id, rd.Distance, g.Special))
                        .ToList()
                        )
                );
         });

        var distancesPoints = raceDistancesWithGeoData
            .GroupBy(rd => rd.Longitude + rd.Latitude)
            .Select(gr =>
            {
                var raceDistance = gr.First();
                var latitude = raceDistance.Latitude!.Value;
                var longitude = raceDistance.Longitude!.Value;

                var race = geoData!.First(r => r.Id == raceDistance.RaceId);

                return new GeoPoint("Feature",
                     new Geometry("Point", new List<decimal>() { latitude, longitude }),
                     new Properties(race.Name, race.NameId, race.StartDate,
                        gr.Select(rd => new RaceDistanceSimple(rd.Id, rd.Distance, race.Special))
                          .ToList()
                          )
                     );
            });

        return racesPoints
            .Concat(distancesPoints)
            .Where(p => !p.Geometry.Coordinates.ToList().SequenceEqual(_emptyPoint))
            .ToList();
    }
}
