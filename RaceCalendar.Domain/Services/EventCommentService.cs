using Microsoft.Extensions.Configuration;
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
    private readonly IMailSender _mailSender;
    private readonly IGetEventQuery _getEventQuery;
    private readonly IConfiguration _config;

    public EventCommentService(
        ICreateEventCommentCommand createEventCommentCommand,
        IGetEventCommentsQuery getEventCommentsQuery,
        IUserService userService,
        IGetEventCommentQuery getEventCommentQuery,
        IDeleteEventCommentCommand deleteEventCommentCommand,
        IUpdateEventCommentCommand updateEventCommentCommand,
        IMailSender mailSender,
        IGetEventQuery getEventQuery,
        IConfiguration config)
    {
        _createEventCommentCommand = createEventCommentCommand ?? throw new ArgumentNullException(nameof(createEventCommentCommand));
        _getEventCommentsQuery = getEventCommentsQuery ?? throw new ArgumentNullException(nameof(getEventCommentsQuery));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _getEventCommentQuery = getEventCommentQuery ?? throw new ArgumentNullException(nameof(getEventCommentQuery));
        _deleteEventCommentCommand = deleteEventCommentCommand ?? throw new ArgumentNullException(nameof(deleteEventCommentCommand));
        _updateEventCommentCommand = updateEventCommentCommand ?? throw new ArgumentNullException(nameof(updateEventCommentCommand));
        _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
        _getEventQuery = getEventQuery ?? throw new ArgumentNullException(nameof(getEventQuery));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task Create(EventComment eventComment)
    {
        await _createEventCommentCommand.Execute(eventComment);

        var @event = await _getEventQuery.QueryAsync(eventComment.EventId);
        var eventCreator = await _userService.GetByIdAsync(@event.CreatedBy);

        var subject = "Получихте нов коментар на ваше груповo бягане";
        var eventPath = $"{_config["EventPath"]}/{@event.Id}";
        var body = $@"
Здравейте, {eventCreator.FirstName} {eventCreator.LastName}!
<br />
<br />
Получихте нов коментар на събитието ""{@event.Name}"", което Вие организирате.
<br />
<br />
<a target=""_blank"" href=""{eventPath}"" style=""border: 1px solid white; padding: 8px; height: 36px; background-color: #71ae72; font-family: Roboto, ""Helvetica Neue"", sans-serif; font-size: 14px; color: white; font-weight: 700; cursor: pointer; text-decoration:none;"">
    Линк към събитието
</a>
<br />
<br />
Поздрави,
<br />
Eкипът на <a href=""http://racecalendar.bg/"" target=""_blank"" >racecalendar.bg</a>
";
        await _mailSender.SendAsync(eventCreator.Email, subject, body);
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
