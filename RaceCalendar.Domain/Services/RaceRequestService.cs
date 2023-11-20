using Microsoft.Extensions.Options;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Helpers;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Options;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Domain.Services;

public class RaceRequestService : IRaceRequestService
{
    private readonly ICreateRaceRequestCommand _createRaceRequestCommand;
    private readonly IOptions<RaceRequestServiceOptions> _options;
    private readonly IMailSender _mailSender;
    private readonly IGetRaceRequestsQuery _getRaceRequestsQuery;
    private readonly IGetRacesByNameIdsQuery _getRacesByNameIdsQuery;
    private readonly IMarkRaceRequestAsProcessedCommand _markRaceRequestAsProcessedCommand;

    public RaceRequestService(
        ICreateRaceRequestCommand createRaceRequestCommand,
        IOptions<RaceRequestServiceOptions> options,
        IMailSender mailSender,
        IGetRaceRequestsQuery getRaceRequestsQuery,
        IGetRacesByNameIdsQuery getRacesByNameIdsQuery,
        IMarkRaceRequestAsProcessedCommand markRaceRequestAsProcessedCommand)
    {
        _createRaceRequestCommand = createRaceRequestCommand ?? throw new ArgumentNullException(nameof(createRaceRequestCommand));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
        _getRaceRequestsQuery = getRaceRequestsQuery ?? throw new ArgumentNullException(nameof(getRaceRequestsQuery));
        _getRacesByNameIdsQuery = getRacesByNameIdsQuery ?? throw new ArgumentNullException(nameof(getRacesByNameIdsQuery));
        _markRaceRequestAsProcessedCommand = markRaceRequestAsProcessedCommand ?? throw new ArgumentNullException(nameof(markRaceRequestAsProcessedCommand));
    }

    public Task Create(RaceRequest raceRequest)
    {
        if (!string.IsNullOrEmpty(raceRequest.ClientIP) &&
            !IPAddressHelper.ValidateIPv4(raceRequest.ClientIP))
        {
            throw new ArgumentException("Not a valid IP address.");
        }

        if (!string.IsNullOrEmpty(raceRequest.Source) &&
            !Uri.IsWellFormedUriString(raceRequest.Source, UriKind.RelativeOrAbsolute))
        {
            throw new ArgumentException("Not a valid source url.");
        }

        return CreateInternal(raceRequest);
    }

    public async Task<IEnumerable<RaceRequest>> GetAll(bool includeProcessed = false)
    {
        var requests = await _getRaceRequestsQuery.Get(includeProcessed);

        var existingRaces = requests
            .Where(r => r.NameId is not null)
            .Select(x => x.NameId!)
            .ToHashSet();
        var races = await _getRacesByNameIdsQuery.Get(existingRaces);

        foreach (var request in requests.Where(x => x.NameId is not null))
        {
            request.OldStartDate = races.TryGetValue(request.NameId!, out var race) ?
                race?.StartDate :
                null;
        }

        return requests.OrderByDescending(r => r.CreatedOn);
    }

    public async Task MarkAsProcessed(int raceRequestId)
    {
        await _markRaceRequestAsProcessedCommand.Execute(raceRequestId);
    }

    private async Task CreateInternal(RaceRequest raceRequest)
    {
        await _createRaceRequestCommand.Execute(raceRequest);

        if (_options.Value.SendEmail)
        {
            _mailSender.SendRequest(raceRequest);
        }
    }
}
