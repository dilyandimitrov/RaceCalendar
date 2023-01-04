using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Api.Requests;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Api.Controllers;

[Route("v1/api/[Controller]")]
[ApiController]
public class RaceAdminController : ControllerBase
{
    private readonly IRaceService _raceService;

    public RaceAdminController(IRaceService raceService)
    {
        _raceService = raceService ?? throw new ArgumentNullException(nameof(raceService));
    }

    [HttpPost]
    public async Task Create([FromBody] RaceUpdateRequest raceRequest)
    {
        var race = new Race(
            raceRequest.Id,
            raceRequest.Name,
            raceRequest.NameId,
            raceRequest.Country,
            raceRequest.City,
            raceRequest.StartDate,
            raceRequest.EndDate,
            raceRequest.Link,
            raceRequest.Tags,
            null,
            (Terrains?)raceRequest.Terrain,
            (Specials?)raceRequest.Special);

        await _raceService.Update(race);
    }
}
