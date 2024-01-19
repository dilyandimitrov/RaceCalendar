using NodaTime;
using RaceCalendar.Domain.Clients;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Services.Interfaces;
using System.Globalization;

namespace RaceCalendar.Domain.Services;

public class EventNotificationService : IEventNotificationService
{
    private readonly IDiscordWebhookClient _discordWebhookClient;
    private readonly IUserService _userService;

    public EventNotificationService(
        IDiscordWebhookClient discordWebhookClient,
        IUserService userService)
    {
        _discordWebhookClient = discordWebhookClient;
        _userService = userService;
    }

    public async Task NotifyNewEvent(Event @event)
    {
        if (@event.CreatedByUser is null)
        {
            var user = await _userService.GetByIdAsync(@event.CreatedBy);
            @event.CreatedByUser = new UserForEvents(@event.CreatedBy, user.FirstName, user.LastName);
        }

        var message = $"Ново груповo бягане - {GetEventLink(@event)} на {GetFormattedDate(@event.StartDate)} " +
            $"(**{@event.CreatedByUser!.FirstName} {@event.CreatedByUser!.LastName}**)";

        await _discordWebhookClient.SendMessageToRaceAddedWebhook(message);
    }

    private static string GetFormattedDate(LocalDate date)
    {
        var dateStr = date.ToString("d MMMM yyyy, dddd", CultureInfo.CreateSpecificCulture("bg-BG"));

        return dateStr;
    }

    private static string GetEventLink(Event @event)
    {
        var link = $"[**{@event.Name}**](<https://racecalendar.bg/event/{@event.Id}>)";

        return link;
    }
}
