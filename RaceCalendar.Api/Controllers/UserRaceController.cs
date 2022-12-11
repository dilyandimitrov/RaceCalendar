using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Api.Requests;
using RaceCalendar.Api.Responses;
using RaceCalendar.Api.Utils;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Api.Controllers;

[Route("v1/api/[controller]")]
[ApiController]
public class UserRaceController : ControllerBase
{
    private readonly IUserRaceService _userRaceService;
    private readonly IUserResultService _userResultService;

    public UserRaceController(IUserRaceService userRaceService, IUserResultService userResultService)
    {
        _userRaceService = userRaceService ?? throw new ArgumentNullException(nameof(userRaceService));
        _userResultService = userResultService ?? throw new ArgumentNullException(nameof(userResultService));
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IEnumerable<UserRaceResponse>> GetByUser()
    {
        var userRacesByUser = await _userRaceService.Get(User.GetUserId());

        return userRacesByUser.Select(x => new UserRaceResponse(
            x.Id!.Value,
            x.RaceId,
            x.RaceDistanceId));
    }

    [Route("races")]
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IEnumerable<UserRace>> GetRacesByUser()
    {
        var userRaces = await _userRaceService.GetAllByUser(User.GetUserId());

        return userRaces;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task Create(UserRaceCreateRequest userRace)
    {
        //if (userRace.RaceDistanceId is null)
        //{
        //    throw new ArgumentException("Distance id should not be null");
        //}

        //userRace.CreatedOn = DateTime.UtcNow;
        //userRace.UserId = User.GetUserId();

        //userRace.Description = $"{raceDistance.NameId};{raceDistance.Distances.First().Distance}";

        //return Ok(new OkApiResponse(_repository.Create(userRace)));

        await _userRaceService.Create(userRace.ToDomainModel(User.GetUserId()));
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task Delete([FromQuery] int id)
    {
        await _userRaceService.Delete(id);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task Update(UserRaceUpdateRequest userRace)
    {
        await _userRaceService.Update(userRace.ToDomainModel(User.GetUserId()));
    }

    [Route("getresult")]
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetResult([FromQuery] string url)
    {
        var userNames = User.GetUniqueName();
        var (position, result, type) = await _userResultService.GetResult(url, userNames);

        if (position is null && result is null)
        {
            return NotFound($"The result for {userNames} cannot be found!");
        }

        return Ok(new { position, result, type });
    }

    [Route("getAllResults")]
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task GetAllResult()
    {
        await _userResultService.FetchAndSaveAllResults(User.GetCurrentUserEmail());
    }
}
