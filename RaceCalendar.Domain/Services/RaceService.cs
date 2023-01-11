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
    private readonly IUpdateRaceInfoCommand _updateRaceInfoCommand;
    private readonly IExcelUpdaterService _excelUpdaterService;
    private readonly IDeleteRaceInfosCommand _deleteRaceInfosCommand;
    private readonly IDeleteRaceDistanceCommand _deleteRaceDistanceCommand;
    private readonly ICreateRaceDistanceCommand _createRaceDistanceCommand;
    private readonly ICreateRaceInfoCommand _createRaceInfoCommand;
    private readonly ISystemInfoService _systemInfoService;

    public RaceService(
        IGetRacesByNameIdsQuery getRacesByNameIdsQuery,
        IGetRacesByRaceIdsQuery getRacesByRaceIdsQuery,
        IGetRaceDistancesQuery getRaceDistancesQuery,
        IGetRaceInfosQuery getRaceInfosQuery,
        IUpdateRaceCommand updateRaceCommand,
        IUpdateRaceDistanceCommand updateRaceDistanceCommand,
        IUpdateRaceInfoCommand updateRaceInfoCommand,
        IExcelUpdaterService excelUpdaterService,
        IDeleteRaceInfosCommand deleteRaceInfosCommand,
        IDeleteRaceDistanceCommand deleteRaceDistanceCommand,
        ICreateRaceDistanceCommand createRaceDistanceCommand,
        ICreateRaceInfoCommand createRaceInfoCommand,
        ISystemInfoService systemInfoService)
    {
        _getRacesByNameIdsQuery = getRacesByNameIdsQuery ?? throw new ArgumentNullException(nameof(getRacesByNameIdsQuery));
        _getRacesByRaceIdsQuery = getRacesByRaceIdsQuery ?? throw new ArgumentNullException(nameof(getRacesByRaceIdsQuery));
        _getRaceDistancesQuery = getRaceDistancesQuery ?? throw new ArgumentNullException(nameof(getRaceDistancesQuery));
        _getRaceInfosQuery = getRaceInfosQuery ?? throw new ArgumentNullException(nameof(getRaceInfosQuery));
        _updateRaceCommand = updateRaceCommand ?? throw new ArgumentNullException(nameof(updateRaceCommand));
        _updateRaceDistanceCommand = updateRaceDistanceCommand ?? throw new ArgumentNullException(nameof(updateRaceDistanceCommand));
        _updateRaceInfoCommand = updateRaceInfoCommand ?? throw new ArgumentNullException(nameof(updateRaceInfoCommand));
        _excelUpdaterService = excelUpdaterService ?? throw new ArgumentNullException(nameof(excelUpdaterService));
        _deleteRaceInfosCommand = deleteRaceInfosCommand ?? throw new ArgumentNullException(nameof(deleteRaceInfosCommand));
        _deleteRaceDistanceCommand = deleteRaceDistanceCommand ?? throw new ArgumentNullException(nameof(deleteRaceDistanceCommand));
        _createRaceDistanceCommand = createRaceDistanceCommand ?? throw new ArgumentNullException(nameof(createRaceDistanceCommand));
        _createRaceInfoCommand = createRaceInfoCommand ?? throw new ArgumentNullException(nameof(createRaceInfoCommand));
        _systemInfoService = systemInfoService ?? throw new ArgumentNullException(nameof(systemInfoService));
    }

    public async Task Update(Race race)
    {
        await _excelUpdaterService.Update(race);

        await _updateRaceCommand.Execute(race);

        await UpdateDistances(race);
        
        await UpdateInfos(race);

        await _systemInfoService.CreateOrUpdateDbLastUpdated();
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

        race.Distances = distances.OrderBy(x => x.Distance);

        var raceInfos = await _getRaceInfosQuery.Get(race.Id, distances.Select(x => x.Id).ToHashSet());

        foreach (RaceDistance distance in race.Distances)
        {
            distance.Info = raceInfos.TryGetValue(distance.Id, out var raceInfo)
                ? raceInfo
                : Enumerable.Empty<RaceInfo>();
        }

        return race;
    }

    private async Task UpdateDistances(Race race)
    {
        var raceDb = await Get(race.Id);

        if (raceDb is null)
        {
            throw new InvalidOperationException($"Race with id {race.Id} doesn't exist");
        }

        var createRaceDistanceTasks = race.Distances
                        .Where(d => !raceDb.Distances.Select(x => x.Id).Contains(d.Id))
                        .Select(d => _createRaceDistanceCommand.Execute(d))
                        .ToList();
        await Task.WhenAll(createRaceDistanceTasks);

        var distanceIdsToDelete = raceDb.Distances
                        .Where(d => !race.Distances.Select(x => x.Id).Contains(d.Id))
                        .Select(x => x.Id)
                        .ToHashSet();
        
        var infoIdsToDelete = raceDb.Distances
                        .Where(d => distanceIdsToDelete.Contains(d.Id) && d.Info is not null)
                        .SelectMany(d => d.Info!.Select(i => i.Id))
                        .ToHashSet();

        await _deleteRaceInfosCommand.Execute(infoIdsToDelete);
        await _deleteRaceDistanceCommand.Execute(distanceIdsToDelete);

        var updateRaceDistanceTasks = race.Distances.Select(distance => _updateRaceDistanceCommand.Execute(distance));
        await Task.WhenAll(updateRaceDistanceTasks);
    }

    private async Task UpdateInfos(Race race)
    {
        var raceDb = await Get(race.Id);

        if (raceDb is null)
        {
            throw new InvalidOperationException($"Race with id {race.Id} doesn't exist");
        }

        var deleteInfoTasks = new List<Task>();
        var createInfoTasks = new List<Task>();
        foreach (var distance in race.Distances)
        {
            var distanceDb = raceDb.Distances.FirstOrDefault(d => d.Id == distance.Id);

            if (distanceDb is not null &&
                distanceDb.Info is not null &&
                distance.Info is not null)
            {
                var createDistanceInfoTasks = distance.Info
                    .Where(info => !distanceDb.Info.Select(x => x.Id).Contains(info.Id))
                    .Select(info => _createRaceInfoCommand.Execute(info))
                    .ToList();
                createInfoTasks.AddRange(createDistanceInfoTasks);

                var idsToDelete = distanceDb.Info
                    .Where(info => !distance.Info.Select(x => x.Id).Contains(info.Id))
                    .Select(x => x.Id)
                    .ToHashSet();

                deleteInfoTasks.Add(_deleteRaceInfosCommand.Execute(idsToDelete));
            }
        }

        await Task.WhenAll(createInfoTasks);
        await Task.WhenAll(deleteInfoTasks);

        var updateInfoTasks = race.Distances
                .Where(distance => distance.Info is not null)
                .SelectMany(distance => distance.Info!
                    .Select(info => _updateRaceInfoCommand.Execute(info)));

        await Task.WhenAll(updateInfoTasks);
    }
}
