namespace RaceCalendar.Domain.Models;

public record RaceInfo(
    int Id,
    int RaceId,
    int RaceDistanceId,
    string Name, 
    string Value);