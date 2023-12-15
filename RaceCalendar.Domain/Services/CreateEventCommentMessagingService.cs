using Microsoft.Extensions.Configuration;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Domain.Services;

public class CreateEventCommentMessagingService : ICreateEventCommentMessagingService
{
    private readonly IConfiguration _config;
    private readonly IMailSender _mailSender;
    private readonly IGetEventQuery _getEventQuery;
    private readonly IUserService _userService;

    public CreateEventCommentMessagingService(
        IConfiguration config,
        IMailSender mailSender,
        IGetEventQuery getEventQuery,
        IUserService userService)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
        _getEventQuery = getEventQuery ?? throw new ArgumentNullException(nameof(getEventQuery));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task SendMessage(EventComment eventComment)
    {
        var @event = await _getEventQuery.QueryAsync(eventComment.EventId);

        if (@event is null)
        {
            return;
        }

        var commentOnYourOwnEvent = @event.CreatedBy == eventComment.CreatedBy;
        if (commentOnYourOwnEvent)
        {
            return;
        }

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
}
