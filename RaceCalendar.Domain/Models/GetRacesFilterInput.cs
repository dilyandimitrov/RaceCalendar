using NodaTime;
using System;

namespace RaceCalendar.Domain.Models;

public record GetRacesFilterInput(
    string? Text,
    string? City,
    LocalDate? FromDate,
    LocalDate? ToDate,
    double? FromDistance,
    double? ToDistance,
    Terrains? Terrain,
    Specials? Special,
    Cancelled? Cancelled,
    int? ShowPrevious);
