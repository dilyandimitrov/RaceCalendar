using System.ComponentModel.DataAnnotations;

namespace RaceCalendar.Domain.Options;

public class RaceRequestServiceOptions
{
    [Required]
    public bool SendEmail { get; set; }
}
