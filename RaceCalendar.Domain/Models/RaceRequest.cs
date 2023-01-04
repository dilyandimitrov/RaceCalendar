using NodaTime;
using System;

namespace RaceCalendar.Domain.Models;

public record RaceRequest
    (int? Id,
    RaceTypes Type,
    string? Name,
    string? NameId,
    LocalDate? StartDate,
    string Text,
    string Source,
    string? ClientIP,
    DateTime? CreatedOn,
    string? ContactInfo,
    bool IsProcessed
    )
{
    public LocalDate? OldStartDate { get; set; }
}
