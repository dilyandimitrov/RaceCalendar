using NodaTime;

namespace RaceCalendar.Domain.Models.Events;

public class Event
{
    public Event(
        long id,
        string name,
        string? description,
        string city,
        decimal latitude,
        decimal longitude,
        LocalDate startDate,
        TimeSpan startTime,
        double? distance,
        int? elevationGain,
        string? link,
        Terrains? terrain,
        Cancelled? cancelled,
        bool isPublic,
        int maxParticipants,
        string? contact,
        string createdBy,
        DateTime createdOn,
        DateTime? modifiedOn,
        long? commentsCount = null,
        long? visitorsCount = null)
    {
        Id = id;
        Name = name;
        Description = description;
        City = city;
        Latitude = latitude;
        Longitude = longitude;
        StartDate = startDate;
        StartTime = startTime;
        Distance = distance;
        ElevationGain = elevationGain;
        Link = link;
        Terrain = terrain;
        Cancelled = cancelled;
        IsPublic = isPublic;
        MaxParticipants = maxParticipants;
        Contact = contact;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
        CommentsCount = commentsCount;
        VisitorsCount = visitorsCount;
    }

    public long Id { get; set; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public string City { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public LocalDate StartDate { get; init; }
    public TimeSpan StartTime { get; init; }
    public double? Distance { get; init; }
    public int? ElevationGain { get; set; }
    public string? Link { get; init; }
    public Terrains? Terrain { get; init; }
    public Cancelled? Cancelled { get; init; }
    public bool IsPublic { get; init; }
    public int MaxParticipants { get; init; }
    public string? Contact { get; init; }
    public string CreatedBy { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? ModifiedOn { get; init; }
    public long? CommentsCount { get; set; }

    public long? VisitorsCount { get; set; }
    public UserForEvents? CreatedByUser { get; set; } = default!;
}
