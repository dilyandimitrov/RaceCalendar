using NodaTime;
using System.Collections.Generic;

namespace RaceCalendar.Domain.Models;

public class Race
{
    public Race(
        int id, 
        string name, 
        string nameId, 
        string? country, 
        string? city,
        LocalDate? startDate,
        LocalDate? endDate,
        string? link, 
        string? tags, 
        Cancelled? cancelled, 
        Terrains? terrain, 
        Specials? special)
    {
        Id = id;
        Name = name;
        NameId = nameId;
        Country = country;
        City = city;
        StartDate = startDate;
        EndDate = endDate;
        Link = link;
        Tags = tags;
        Cancelled = cancelled;
        Terrain = terrain;
        Special = special;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string NameId { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public LocalDate? StartDate { get; set; }
    public LocalDate? EndDate { get; set; }
    public string? Link { get; set; }
    public string? Tags { get; set; }
    public Cancelled? Cancelled { get; set; }
    public Terrains? Terrain { get; set; }
    public Specials? Special { get; set; }

    public IEnumerable<RaceDistance> Distances { get; set; } = default!;
    public bool AllDistances { get; set; }

    public bool HasPassed
    {
        get
        {
            if (StartDate is null)
            {
                return false;
            }

            Instant now = SystemClock.Instance.GetCurrentInstant();
            DateTimeZone bulgariaTimeZone = DateTimeZoneProviders.Tzdb["Europe/Sofia"];
            ZonedDateTime nowInBulgaria = now.InZone(bulgariaTimeZone);

            return StartDate.Value.ToDateTimeUnspecified() < nowInBulgaria.ToDateTimeUnspecified();
        }
    }

    public bool IsDateConfirmed => true;
}
