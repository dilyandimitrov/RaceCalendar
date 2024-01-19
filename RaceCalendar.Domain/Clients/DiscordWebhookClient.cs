using Microsoft.Extensions.Options;
using RaceCalendar.Domain.Options;
using System.Text;

namespace RaceCalendar.Domain.Clients;

public class DiscordWebhookClient : IDiscordWebhookClient
{
    private readonly HttpClient _httpClient;
    private readonly DiscordOptions _discordOptions;

    public DiscordWebhookClient(
        HttpClient httpClient,
        IOptions<DiscordOptions> discordOptions)
    {
        _httpClient = httpClient;
        _discordOptions = discordOptions.Value;
    }

    public Task SendMessageToRaceChangedWebhook(string message)
    {
        if (!_discordOptions.EnableNotification)
        {
            return Task.CompletedTask;
        }

        return _httpClient.PostAsync(_discordOptions.ChangesWebhookUrl, PrepareHttpContent(message));
    }

    public Task SendMessageToRaceAddedWebhook(string message)
    {
        if (!_discordOptions.EnableNotification)
        {
            return Task.CompletedTask;
        }

        return _httpClient.PostAsync(_discordOptions.AdditionsWebhookUrl, PrepareHttpContent(message));
    }

    private static HttpContent PrepareHttpContent(string message)
    {
        string content = "{\"content\": \"" + message + "\"}";
        return new StringContent(content, Encoding.UTF8, "application/json");
    }
}
