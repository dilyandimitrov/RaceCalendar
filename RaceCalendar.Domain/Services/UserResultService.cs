using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Models.Authentication;
using RaceCalendar.Domain.Services.Interfaces;
using RaceCalendar.Domain.Strategies.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services;

public class UserResultService : IUserResultService
{
    private readonly IList<FetchResultStrategy> _strategies = new List<FetchResultStrategy>();
    private readonly ITransliterationService _transliterationService;
    private readonly IUserService _userService;
    private readonly IUserRaceService _userRaceService;

    public UserResultService(
        ITransliterationService transliterationService,
        IUserService userService,
        IUserRaceService userRaceService)
    {
        _strategies.Add(new TrailSeriesFetchResultStrategy());
        _strategies.Add(new TrackSportLiveFetchResultStrategy());
        _strategies.Add(new FiveKmRunFetchResultStrategy());
        _strategies.Add(new RaceTrackingFetchResultStrategy());
        _strategies.Add(new LudMarathonFetchResultStrategy());

        _transliterationService = transliterationService ?? throw new ArgumentNullException(nameof(transliterationService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _userRaceService = userRaceService ?? throw new ArgumentNullException(nameof(userRaceService));
    }

    public async Task<(string? position, string? result, ResultTypes? type)> GetResult(string resultUrl, string userNames)
    {
        var namesSplit = userNames.Trim().Split(' ');

        if (namesSplit.Length < 2)
        {
            return (null, null, null);
        }

        var firstName = namesSplit[0];
        var lastName = namesSplit[1];

        var strategy = _strategies.SingleOrDefault(s => s.CanApply(resultUrl));

        if (strategy == null)
        {
            return (null, null, null);
        }

        var (position, result, type) = await GetResultInternal(strategy, firstName, lastName, resultUrl);

        if (position is null && result is null)
        {
            var transliteratedFirstName = _transliterationService.GetWord(firstName);
            var transliteratedLastName = _transliterationService.GetWord(lastName);

            (position, result, type) = await GetResultInternal(strategy, transliteratedFirstName, transliteratedLastName, resultUrl);
        }

        return (position, result, type);
    }

    public async Task FetchAndSaveAllResults(string userEmail)
    {
        var user = await _userService.GetByEmailAsync(userEmail);
        var userRaces = await _userRaceService.GetAllByUser(user.Id);

        var tasks = userRaces
            .Where(x => x.RaceDistance is not null && x.RaceDistance.ResultsLink is not null && x.Result is null)
            .Select(x => FetchAndSaveResult(x, $"{user.FirstName} {user.LastName}"))
            .ToList();

        await Task.WhenAll(tasks);
    }

    private async Task FetchAndSaveResult(UserRace userRace, string userNames)
    {
        var (position, result, type) = await GetResult(userRace.RaceDistance!.ResultsLink, userNames);
        if (position is not null && result is not null && type is not null)
        {
            var userRaceForUpdate = new UserRace(
                userRace.Id,
                userRace.UserId,
                userRace.Type,
                userRace.RaceId,
                userRace.RaceDistanceId,
                null,
                result,
                type,
                position);

            await _userRaceService.Update(userRaceForUpdate);
        }
    }

    private async Task<(string? position, string? time, ResultTypes? type)> GetResultInternal(FetchResultStrategy strategy, string firstName, string lastName, string resultUrl)
    {
        try
        {
            var result = await strategy.FetchResults(firstName, lastName, resultUrl);
            return result;
        }
        catch (Exception)
        {
            return (null, null, null);
        }
    }
}
