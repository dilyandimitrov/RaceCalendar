using NodaTime;
using RaceCalendar.Domain.Clients;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Services.Interfaces;
using System.Globalization;

namespace RaceCalendar.Domain.Services;

public class RaceNotificationService : IRaceNotificationService
{
    private readonly IDiscordWebhookClient _discordWebhookClient;

    public RaceNotificationService(IDiscordWebhookClient discordWebhookClient)
    {
        _discordWebhookClient = discordWebhookClient;
    }

    public Task NotifyChange(Race oldRace, Race newRace)
    {
        if (newRace.StartDate != oldRace.StartDate && newRace.StartDate.HasValue)
        {
            var dateStr = GetFormattedDate(newRace.StartDate.Value);
            var message = $"{GetRaceLink(newRace)} - нова дата - {dateStr}";
            return _discordWebhookClient.SendMessageToRaceChangedWebhook(message);
        }

        if (newRace.Cancelled == Cancelled.Cancelled && oldRace.Cancelled == 0)
        {
            var message = $"{GetRaceLink(newRace)} е отменено";
            return _discordWebhookClient.SendMessageToRaceChangedWebhook(message);
        }

        if (oldRace.Cancelled == Cancelled.Cancelled && newRace.Cancelled == 0)
        {
            var message = $"{GetRaceLink(newRace)} вече не е отменено";
            return _discordWebhookClient.SendMessageToRaceChangedWebhook(message);
        }

        return Task.CompletedTask;
    }

    public async Task NotifyNewRace(Race race)
    {
        var message = $"Ново състезание - {GetRaceLink(race)} на {GetFormattedDate(race.StartDate!.Value)}";
        await _discordWebhookClient.SendMessageToRaceAddedWebhook(message);
    }

    private static string GetFormattedDate(LocalDate date)
    {
        var dateStr = date.ToString("d MMMM yyyy, dddd", CultureInfo.CreateSpecificCulture("bg-BG"));

        return dateStr;
    }

    private static string GetRaceLink(Race race)
    {
        var link = $"[**{race.Name}**](<https://racecalendar.bg/run-race/{race.NameId}>)";

        return link;
    }
}
