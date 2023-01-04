using NodaTime;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services;

public class UserRaceService : IUserRaceService
{
    private readonly IGetUserRacesByUserQuery _getUserRacesByUserQuery;
    private readonly IGetRacesByRaceIdsQuery _getRacesByRaceIdsQuery;
    private readonly IGetRaceDistancesQuery _getRaceDistancesQuery;
    private readonly IDeleteUserRaceCommand _deleteUserRaceCommand;
    private readonly ICreateUserRaceCommand _createUserRaceCommand;
    private readonly IUpdateUserRaceCommand _updateUserRaceCommand;
    private readonly IRaceService _raceService;

    public UserRaceService(
        IGetUserRacesByUserQuery getUserRacesByUserQuery,
        IGetRacesByRaceIdsQuery getRacesByRaceIdsQuery,
        IGetRaceDistancesQuery getRaceDistancesQuery,
        IDeleteUserRaceCommand deleteUserRaceCommand,
        ICreateUserRaceCommand createUserRaceCommand,
        IUpdateUserRaceCommand updateUserRaceCommand,
        IRaceService raceService)
    {
        _getUserRacesByUserQuery = getUserRacesByUserQuery ?? throw new ArgumentNullException(nameof(getUserRacesByUserQuery));
        _getRacesByRaceIdsQuery = getRacesByRaceIdsQuery ?? throw new ArgumentNullException(nameof(getRacesByRaceIdsQuery));
        _getRaceDistancesQuery = getRaceDistancesQuery ?? throw new ArgumentNullException(nameof(getRaceDistancesQuery));
        _deleteUserRaceCommand = deleteUserRaceCommand ?? throw new ArgumentNullException(nameof(deleteUserRaceCommand));
        _createUserRaceCommand = createUserRaceCommand ?? throw new ArgumentNullException(nameof(createUserRaceCommand));
        _updateUserRaceCommand = updateUserRaceCommand ?? throw new ArgumentNullException(nameof(updateUserRaceCommand));
        _raceService = raceService ?? throw new ArgumentNullException(nameof(raceService));
    }

    public async Task Create(UserRace userRace)
    {
        //TODO fetch result

        userRace.Description = await GetDescription(userRace);

        await _createUserRaceCommand.Execute(userRace);
    }

    public async Task<IEnumerable<UserRace>> Get(string userId)
    {
        var userRaces = await _getUserRacesByUserQuery.Get(userId);

        return userRaces;
    }

    public async Task<IEnumerable<UserRace>> GetAllByUser(string userId)
    {
        var userRaces = (await _getUserRacesByUserQuery.Get(userId)).ToList();

        var races = await _getRacesByRaceIdsQuery.Get(userRaces.Select(x => x.RaceId).ToHashSet());

        foreach (var userRace in userRaces)
        {
            if (races.TryGetValue(userRace.RaceId, out var race) && race is not null)
            {
                userRace.Race = race;
            }

            if (userRace.RaceDistanceId is not null)
            {
                var raceDistance = await _getRaceDistancesQuery
                    .Get(userRace.RaceId, new HashSet<int>() { userRace.RaceDistanceId.Value });

                userRace.Race.Distances = raceDistance;
                userRace.RaceDistance = raceDistance.FirstOrDefault();
            }
        }

        userRaces.Sort((r1, r2) => DateTime.Compare(
            r1.Race!.StartDate is not null ? r1.Race.StartDate.Value.ToDateTimeUnspecified() : DateTime.MinValue,
            r2.Race!.StartDate is not null ? r2.Race!.StartDate.Value.ToDateTimeUnspecified() : DateTime.MinValue));

        return userRaces;
    }

    public async Task Update(UserRace userRace)
    {
        userRace.Description = await GetDescription(userRace);

        await _updateUserRaceCommand.Execute(userRace);
    }

    public async Task Delete(int id)
    {
        await _deleteUserRaceCommand.Execute(id);
    }

    private async Task<string?> GetDescription(UserRace userRace)
    {
        if (userRace.RaceDistanceId is not null)
        {
            var race = await _raceService.Get(userRace.RaceId, new HashSet<int>() { userRace.RaceDistanceId.Value });
            return race is not null ?
                $"{race.NameId};{race.Distances.First().Distance}" :
                null;
        }

        return null;
    }
}
