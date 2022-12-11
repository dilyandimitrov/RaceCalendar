using NodaTime;
using System;
using System.Collections.Generic;

namespace RaceCalendar.Domain.Models;

public class Race
{
    public Race(
        int id, 
        string name, 
        string nameId, 
        string country, 
        string city, 
        DateTime? startDate, 
        DateTime? endDate,
        string link, 
        string tags, 
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

    public int Id { get; init; }
    public string Name { get; init; }
    public string NameId { get; init; }
    public string Country { get; init; }
    public string City { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string Link { get; init; }
    public string Tags { get; set; }
    public Cancelled? Cancelled { get; init; }
    public Terrains? Terrain { get; init; }
    public Specials? Special { get; init; }

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

            return StartDate.Value < nowInBulgaria.ToDateTimeUnspecified();
        }
    }

    public bool IsDateConfirmed => true;
}
