namespace RaceCalendar.Domain.Clients;

public interface IDiscordWebhookClient
{
    Task SendMessageToRaceChangedWebhook(string message);
    Task SendMessageToRaceAddedWebhook(string message);
}
