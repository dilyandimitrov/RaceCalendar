using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;
using System.Security;

namespace RaceCalendar.Domain.Services;

public class EventService : IEventService
{
    private readonly ICreateEventCommand _createEventCommand;
    private readonly IUpdateEventCommand _updateEventCommand;
    private readonly IDeleteEventCommand _deleteEventCommand;
    private readonly IGetEventQuery _getEventQuery;
    private readonly IUserService _userService;

    public EventService(
        ICreateEventCommand createEventCommand,
        IUpdateEventCommand updateEventCommand,
        IDeleteEventCommand deleteEventCommand,
        IGetEventQuery getEventQuery,
        IUserService userService)
    {
        _createEventCommand = createEventCommand ?? throw new ArgumentNullException(nameof(createEventCommand));
        _updateEventCommand = updateEventCommand ?? throw new ArgumentNullException(nameof(updateEventCommand));
        _deleteEventCommand = deleteEventCommand ?? throw new ArgumentNullException(nameof(deleteEventCommand));
        _getEventQuery = getEventQuery ?? throw new ArgumentNullException(nameof(getEventQuery));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task Create(Event @event)
    {
        await _createEventCommand.Execute(@event);
    }

    public async Task Delete(long id, string userId)
    {
        var @event = await _getEventQuery.QueryAsync(id);
        var user = await _userService.GetByIdAsync(userId);

        if (@event.CreatedBy != userId && !user.IsAdmin)
        {
            throw new SecurityException();
        }

        await _deleteEventCommand.Execute(id);
    }

    public async Task<Event> Get(long id)
    {
        var @event = await _getEventQuery.QueryAsync(id);

        var user = await _userService.GetByIdAsync(@event.CreatedBy);

        @event.CreatedByUser = new UserForEvents(user.Id, user.FirstName, user.LastName);

        return @event;
    }

    public async Task Update(Event @event)
    {
        await _updateEventCommand.Execute(@event);
    }
}
