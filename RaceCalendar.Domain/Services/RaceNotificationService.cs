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
        var messages = new List<string>();

        if (newRace.StartDate != oldRace.StartDate && newRace.StartDate.HasValue)
        {
            var dateStr = GetFormattedDate(newRace.StartDate.Value);
            messages.Add($"{GetRaceLink(newRace)} има нова дата - {dateStr}");
        }

        if (newRace.Longitude != oldRace.Longitude || 
            newRace.Latitude != oldRace.Latitude ||
            newRace.Distances.Any(newDistance =>
            {
                var oldDistance = oldRace.Distances.SingleOrDefault(oldDistance => oldDistance.Id == newDistance.Id);
                return
                    oldDistance?.Longitude != newDistance.Longitude ||
                    oldDistance?.Latitude != newDistance.Latitude;
            }))
        {
            messages.Add($"{GetRaceLink(newRace)} има нова начална точка на тръгване");
        }

        if (newRace.Cancelled == Cancelled.Cancelled && oldRace.Cancelled == 0)
        {
            messages.Add($"{GetRaceLink(newRace)} е отменено");
        }

        if (oldRace.Cancelled == Cancelled.Cancelled && newRace.Cancelled == 0)
        {
            messages.Add($"{GetRaceLink(newRace)} вече не е отменено");
        }

        if (newRace.Distances.Any(newDistance =>
        {
            var oldDistance = oldRace.Distances.SingleOrDefault(oldDistance => oldDistance.Id == newDistance.Id);
            return !string.IsNullOrEmpty(newDistance.ResultsLink) && oldDistance?.ResultsLink != newDistance.ResultsLink;
        }))
        {
            messages.Add($"{GetRaceLink(newRace)} има добавени резултати");
        }

        if (newRace.Distances.Any(newDistance =>
        {
            var oldDistance = oldRace.Distances.SingleOrDefault(oldDistance => oldDistance.Id == newDistance.Id);
            return oldDistance is null || newDistance.Distance != oldDistance.Distance;
        }))
        {
            messages.Add($"{GetRaceLink(newRace)} има нова дистанция");
        }

        if (messages.Any())
        {
            return _discordWebhookClient.SendMessageToRaceChangedWebhook(string.Join("\\n", messages));
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
