using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;
using System.Security;

namespace RaceCalendar.Domain.Services;

public class EventCommentService : IEventCommentService
{
    private readonly ICreateEventCommentCommand _createEventCommentCommand;
    private readonly IGetEventCommentsQuery _getEventCommentsQuery;
    private readonly IUserService _userService;
    private readonly IGetEventCommentQuery _getEventCommentQuery;
    private readonly IDeleteEventCommentCommand _deleteEventCommentCommand;
    private readonly IUpdateEventCommentCommand _updateEventCommentCommand;
    private readonly ICreateEventCommentMessagingService _createEventCommentMessagingService;

    public EventCommentService(
        ICreateEventCommentCommand createEventCommentCommand,
        IGetEventCommentsQuery getEventCommentsQuery,
        IUserService userService,
        IGetEventCommentQuery getEventCommentQuery,
        IDeleteEventCommentCommand deleteEventCommentCommand,
        IUpdateEventCommentCommand updateEventCommentCommand,
        ICreateEventCommentMessagingService createEventCommentMessagingService)
    {
        _createEventCommentCommand = createEventCommentCommand ?? throw new ArgumentNullException(nameof(createEventCommentCommand));
        _getEventCommentsQuery = getEventCommentsQuery ?? throw new ArgumentNullException(nameof(getEventCommentsQuery));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _getEventCommentQuery = getEventCommentQuery ?? throw new ArgumentNullException(nameof(getEventCommentQuery));
        _deleteEventCommentCommand = deleteEventCommentCommand ?? throw new ArgumentNullException(nameof(deleteEventCommentCommand));
        _updateEventCommentCommand = updateEventCommentCommand ?? throw new ArgumentNullException(nameof(updateEventCommentCommand));
        _createEventCommentMessagingService = createEventCommentMessagingService ?? throw new ArgumentNullException(nameof(createEventCommentMessagingService));
    }

    public async Task Create(EventComment eventComment)
    {
        await _createEventCommentCommand.Execute(eventComment);

        await _createEventCommentMessagingService.SendMessage(eventComment);
    }

    public async Task Update(EventComment eventComment)
    {
        await _updateEventCommentCommand.Execute(eventComment);
    }

    public async Task Delete(long id, string userId)
    {
        var eventComment = await _getEventCommentQuery.QueryAsync(id);
        var user = await _userService.GetByIdAsync(userId);

        if (eventComment.CreatedBy != userId && !user.IsAdmin)
        {
            throw new SecurityException();
        }

        await _deleteEventCommentCommand.Execute(id);
    }

    public async Task<IEnumerable<EventComment>> GetCommentsByEvent(long eventId)
    {
        var comments = await _getEventCommentsQuery.QueryAsync(eventId);

        var users = (await _userService.GetAll()).ToDictionary(k => k.Id, v => v);

        foreach (EventComment comment in comments)
        {
            var user = users[comment.CreatedBy];
            comment.CreatedByUser = new UserForEvents(user.Id, user.FirstName, user.LastName);
        }

        return comments.ToList();
    }
}
