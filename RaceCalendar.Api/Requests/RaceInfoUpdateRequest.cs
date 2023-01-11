namespace RaceCalendar.Api.Requests;

public record RaceInfoUpdateRequest(
    int Id,
    int RaceId,
    int RaceDistanceId,
    string Name,
    string? Value);
