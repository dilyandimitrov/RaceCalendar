using NodaTime;

namespace RaceCalendar.Api.Requests;

public record RaceUpdateRequest(
    int Id,
    string Name,
    string NameId,
    string? Country,
    string? City,
    LocalDate? StartDate,
    LocalDate? EndDate,
    string? Link,
    string? Tags,
    int? Cancelled,
    int? Terrain,
    int? Special,
    IEnumerable<RaceDistanceUpdateRequest> Distances);
