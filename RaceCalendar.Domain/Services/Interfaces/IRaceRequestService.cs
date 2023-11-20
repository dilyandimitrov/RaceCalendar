using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IRaceRequestService
{
    Task Create(RaceRequest raceRequest);
    Task<IEnumerable<RaceRequest>> GetAll(bool includeProcessed = false);
    Task MarkAsProcessed(int raceRequestId);
}
