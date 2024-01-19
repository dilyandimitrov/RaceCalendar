using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IRaceNotificationService
{
    Task NotifyChange(Race oldRace, Race newRace);
    Task NotifyNewRace(Race race);
}
