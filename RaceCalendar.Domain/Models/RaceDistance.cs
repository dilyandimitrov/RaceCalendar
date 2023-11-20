using NodaTime;

namespace RaceCalendar.Domain.Models;

public class RaceDistance
{
    public RaceDistance(
        int id,
        int raceId,
        string? name,
        double? distance,
        LocalDate? startDate,
        TimeSpan? startTime,
        int? unconfirmedDate,
        int? elevationGain,
        string? price,
        string? link,
        string? resultsLink,
        decimal? latitude = null,
        decimal? longitude = null)
    {
        Id = id;
        RaceId = raceId;
        Name = name;
        Distance = distance;
        StartDate = startDate;
        StartTime = startTime;
        UnconfirmedDate = unconfirmedDate;
        ElevationGain = elevationGain;
        Price = price;
        Link = link;
        ResultsLink = resultsLink;
        Latitude = latitude;
        Longitude = longitude;
    }

    public int Id { get; set; }
    public int RaceId { get; set; }
    public string? Name { get; set; }
    public double? Distance { get; set; }
    public LocalDate? StartDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public int? UnconfirmedDate { get; set; }
    public int? ElevationGain { get; set; }
    public string? Price { get; set; }
    public string? Link { get; set; }
    public string? ResultsLink { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public IEnumerable<RaceInfo>? Info { get; set; } = default!;
    public bool IsDateConfirmed => true;
}
