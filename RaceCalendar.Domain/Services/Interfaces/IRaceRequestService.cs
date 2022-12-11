using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IRaceRequestService
{
    Task Create(RaceRequest raceRequest);
    Task<IEnumerable<RaceRequest>> GetAll(bool includeProcessed = false);
    Task MarkAsProcessed(int raceRequestId);
}
