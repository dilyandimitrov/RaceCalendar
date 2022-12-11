using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Api.Controllers;

[Route("v1/api/[Controller]")]
[ApiController]
public class RaceRequestController : ControllerBase
{
    private readonly IRaceRequestService _raceRequestService;

    public RaceRequestController(IRaceRequestService raceRequestService)
    {
        _raceRequestService = raceRequestService ?? throw new ArgumentNullException(nameof(raceRequestService));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]RaceRequest raceRequest)
    {
        await _raceRequestService.Create(raceRequest);

        return Ok();
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SearchSecure([FromQuery] bool showProcessed)
    {
        var requests = await _raceRequestService.GetAll(showProcessed);

        return Ok(requests);
    }

    [HttpPut("process")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> MarkAsProcessed([FromQuery] int id)
    {
        await _raceRequestService.MarkAsProcessed(id);

        return Ok();
    }
}
