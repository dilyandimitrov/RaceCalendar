using System;

namespace RaceCalendar.Domain.Models;

public record GetRacesFilterInput(
    string? Text,
    string? City,
    DateTime? FromDate,
    DateTime? ToDate,
    double? FromDistance,
    double? ToDistance,
    Terrains? Terrain,
    Specials? Special,
    Cancelled? Cancelled,
    int? ShowPrevious);
