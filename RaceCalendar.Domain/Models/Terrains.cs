namespace RaceCalendar.Domain.Models;

[Flags]
public enum Terrains
{
    Trail = 1,
    Asphalt = 2,
    Park = 4,
    Hall = 8,
    Sand = 16,
    Track = 32
}