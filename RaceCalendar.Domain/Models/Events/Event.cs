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
        DateTime? modifiedOn)
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
    }

    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string City { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public LocalDate StartDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public double? Distance { get; set; }
    public int? ElevationGain { get; set; }
    public string? Link { get; set; }
    public Terrains? Terrain { get; set; }
    public Cancelled? Cancelled { get; set; }
    public bool IsPublic { get; set; }
    public int MaxParticipants { get; set; }
    public string? Contact { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public UserForEvents? CreatedByUser { get; set; } = default!;
}
