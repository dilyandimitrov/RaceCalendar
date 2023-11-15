using NodaTime;
using RaceCalendar.Domain.Models;

namespace RaceCalendar.Api.Requests;

public record CreateEventRequest(
    long Id,
    string Name,
    string? Description,
    string City,
    decimal Latitude,
    decimal Longitude,
    LocalDate StartDate,
    TimeOnly StartTime,
    double? Distance,
    int? ElevationGain,
    string? Link,
    Terrains? Terrain,
    Cancelled? Cancelled,
    bool IsPublic,
    int MaxParticipants,
    string? Contact);
