using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using RaceCalendar.Domain.Commands;

namespace RaceCalendar.Domain.Services;

public class RaceService : IRaceService
{
    private readonly IGetRacesByNameIdsQuery _getRacesByNameIdsQuery;
    private readonly IGetRacesByRaceIdsQuery _getRacesByRaceIdsQuery;
    private readonly IGetRaceDistancesQuery _getRaceDistancesQuery;
    private readonly IGetRaceInfosQuery _getRaceInfosQuery;
    private readonly IUpdateRaceCommand _updateRaceCommand;
    private readonly IUpdateRaceDistanceCommand _updateRaceDistanceCommand;
    private readonly IExcelUpdaterService _excelUpdaterService;

    public RaceService(
        IGetRacesByNameIdsQuery getRacesByNameIdsQuery,
        IGetRacesByRaceIdsQuery getRacesByRaceIdsQuery,
        IGetRaceDistancesQuery getRaceDistancesQuery,
        IGetRaceInfosQuery getRaceInfosQuery,
        IUpdateRaceCommand updateRaceCommand,
        IUpdateRaceDistanceCommand updateRaceDistanceCommand,
        IExcelUpdaterService excelUpdaterService)
    {
        _getRacesByNameIdsQuery = getRacesByNameIdsQuery ?? throw new ArgumentNullException(nameof(getRacesByNameIdsQuery));
        _getRacesByRaceIdsQuery = getRacesByRaceIdsQuery ?? throw new ArgumentNullException(nameof(getRacesByRaceIdsQuery));
        _getRaceDistancesQuery = getRaceDistancesQuery ?? throw new ArgumentNullException(nameof(getRaceDistancesQuery));
        _getRaceInfosQuery = getRaceInfosQuery ?? throw new ArgumentNullException(nameof(getRaceInfosQuery));
        _updateRaceCommand = updateRaceCommand ?? throw new ArgumentNullException(nameof(updateRaceCommand));
        _updateRaceDistanceCommand = updateRaceDistanceCommand ?? throw new ArgumentNullException(nameof(updateRaceDistanceCommand));
        _excelUpdaterService = excelUpdaterService ?? throw new ArgumentNullException(nameof(excelUpdaterService));
    }

    public async Task Update(Race race)
    {
        await _excelUpdaterService.Update(race);

        await _updateRaceCommand.Execute(race);

        if (race.Distances.Any())
        {
            var raceDistanceTasks = race.Distances.Select(distance => _updateRaceDistanceCommand.Execute(distance));
            
            await Task.WhenAll(raceDistanceTasks);
        }
    }

    public async Task<Race?> Get(string nameId, ISet<int>? distanceIds = null)
    {
        var raceMap = (await _getRacesByNameIdsQuery.Get(new HashSet<string>() { nameId.ToLower() })).SingleOrDefault();

        return await GetInternal(raceMap.Value, distanceIds);
    }

    public async Task<Race?> Get(int raceId, ISet<int>? distanceIds = null)
    {
        var raceMap = (await _getRacesByRaceIdsQuery.Get(new HashSet<int>() { raceId })).SingleOrDefault();

        return await GetInternal(raceMap.Value, distanceIds);
    }

    private async Task<Race?> GetInternal(Race race, ISet<int>? distanceIds = null)
    {
        if (race is null)
        {
            return null;
        }

        var distances = await _getRaceDistancesQuery.Get(race.Id, distanceIds);

        race.Distances = distances;

        var raceInfos = await _getRaceInfosQuery.Get(race.Id, distances.Select(x => x.Id).ToHashSet());

        foreach (RaceDistance distance in race.Distances)
        {
            distance.Info = raceInfos.TryGetValue(distance.Id, out var raceInfo)
                ? raceInfo
                : Enumerable.Empty<RaceInfo>();
        }

        return race;
    }
}
