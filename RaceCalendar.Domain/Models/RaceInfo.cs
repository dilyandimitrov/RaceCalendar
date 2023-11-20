namespace RaceCalendar.Domain.Models;

public class RaceInfo
{
    public RaceInfo(
        int id,
        int raceId,
        int raceDistanceId,
        string name,
        string? value)
    {
        Id = id;
        RaceId = raceId;
        RaceDistanceId = raceDistanceId;
        Name = name;
        Value = value;
    }

    public int Id { get; set; }
    public int RaceId { get; set; }
    public int RaceDistanceId { get; set; }
    public string Name { get; set; }
    public string? Value { get; set; }
}
