using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Domain.Services;

public class SearchEventsService : ISearchEventsService
{
    private readonly IGetAllUpcomingEventsQuery _getAllUpcomingEventsQuery;
    private readonly IUserService _userService;

    public SearchEventsService(
        IGetAllUpcomingEventsQuery getAllUpcomingEventsQuery,
        IUserService userService)
    {
        _getAllUpcomingEventsQuery = getAllUpcomingEventsQuery ?? throw new ArgumentNullException(nameof(getAllUpcomingEventsQuery));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<IEnumerable<Event>> GetUpcomingEvents(bool showPublicOnly = false)
    {
        var events = await _getAllUpcomingEventsQuery.QueryAsync(showPublicOnly);

        var users = (await _userService.GetAll()).ToDictionary(k => k.Id, v => v);

        foreach (Event @event in events)
        {
            var user = users[@event.CreatedBy];
            @event.CreatedByUser = new UserForEvents(user.Id, user.FirstName, user.LastName);
        }

        return events;
    }
}
