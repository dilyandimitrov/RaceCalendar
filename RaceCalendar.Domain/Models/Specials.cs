using System;

namespace RaceCalendar.Domain.Models;

[Flags]
public enum Specials
{
    Relay = 1,
    TimeRace = 2,
    Team = 4,
    LastManStanding = 8,
    Itra = 16,
    Bfla = 32,
    BSF = 64
}