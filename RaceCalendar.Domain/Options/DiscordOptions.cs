using System.ComponentModel.DataAnnotations;

namespace RaceCalendar.Domain.Options;

public class DiscordOptions
{
    [Required]
    public bool EnableNotification { get; set; } = default;

    [Required]
    public string ChangesWebhookUrl { get; set; } = default!;

    [Required]
    public string AdditionsWebhookUrl { get; set; } = default!;
}
