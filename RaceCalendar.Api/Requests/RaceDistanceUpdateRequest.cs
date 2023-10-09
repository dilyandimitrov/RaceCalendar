using NodaTime;

namespace RaceCalendar.Api.Requests;

public record RaceDistanceUpdateRequest(
    int Id,
    double? Distance,
    string? Name,
    LocalDate StartDate,
    string? StartTime,
    int? ELevationGain,
    string? Link,
    string? ResultsLink,
    decimal? Latitude,
    decimal? Longitude,
    IEnumerable<RaceInfoUpdateRequest>? Info);