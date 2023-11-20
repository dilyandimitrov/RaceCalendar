using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Api.Requests;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Api.Controllers;

[Route("v1/api/[Controller]")]
[ApiController]
[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            (Cancelled?)raceRequest.Cancelled,
            (Terrains?)raceRequest.Terrain,
            (Specials?)raceRequest.Special,
            raceRequest.Latitude,
            raceRequest.Longitude);

        race.Distances = raceRequest.Distances
            .Select(d =>
            {
                var distance = new RaceDistance(
                    d.Id,
                    race.Id,
                    d.Name,
                    d.Distance,
                    d.StartDate,
                    TimeSpan.TryParse(d.StartTime, out var startTime) ? startTime : null,
                    null,
                    d.ELevationGain,
                    string.Empty,
                    d.Link,
                    d.ResultsLink,
                    d.Latitude,
                    d.Longitude);

                distance.Info = d.Info?.Select(i => new RaceInfo(i.Id, i.RaceId, i.RaceDistanceId, i.Name, i.Value)).ToList();

                return distance;
            })
            .ToList();

        await _raceService.Update(race);
    }

    [HttpDelete]
    [Route("{raceId}")]
    public async Task Delete([FromRoute] int raceId)
    {
        await _raceService.Delete(raceId);
    }
}
