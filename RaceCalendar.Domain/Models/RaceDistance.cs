using System;
using System.Collections.Generic;

namespace RaceCalendar.Domain.Models;

public class RaceDistance
{
    public RaceDistance(
        int id,
        int raceId,
        string name,
        double? distance,
        DateTime? startDate,
        TimeSpan? startTime,
        int? unconfirmedDate,
        int? elevationGain,
        string price,
        string link,
        string resultsLink)
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
    }

    public int Id { get; set; }
    public int RaceId { get; set; }
    public string Name { get; set; }
    public double? Distance { get; set; }
    public DateTime? StartDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public int? UnconfirmedDate { get; set; }
    public int? ElevationGain { get; set; }
    public string Price { get; set; }
    public string Link { get; set; }
    public string ResultsLink { get; set; }

    public IEnumerable<RaceInfo> Info { get; set; } = default!;
    public bool IsDateConfirmed => true;
}
