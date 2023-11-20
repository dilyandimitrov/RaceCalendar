using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Api.Controllers;

[Route("v1/api/[Controller]")]
[ApiController]
public class RaceController : ControllerBase
{
    private readonly ISearchRacesService _searchRacesService;
    private readonly IGeoDataService _geoDataService;
    private readonly IRaceService _raceService;

    public RaceController(
        ISearchRacesService searchRacesService,
        IRaceService raceService,
        IGeoDataService geoDataService)
    {
        _searchRacesService = searchRacesService ?? throw new ArgumentNullException(nameof(searchRacesService));
        _raceService = raceService ?? throw new ArgumentNullException(nameof(raceService));
        _geoDataService = geoDataService ?? throw new ArgumentNullException(nameof(geoDataService));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] GetRacesFilterInput criteria,
        [FromQuery] int page,
        [FromQuery] int pageSize)
    {
        var result = await _searchRacesService.Search(criteria, page, pageSize);

        return Ok(result);
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetByDistances(
        [FromRoute] string id,
        [FromQuery] string? distances)
    {
        List<int>? distanceIds = null;
        if (!string.IsNullOrEmpty(distances))
        {
            distanceIds = distances.Trim(',', ' ').Split(",").Select(int.Parse).ToList();
        }

        var result = await _raceService.Get(id, distanceIds?.ToHashSet());

        return Ok(result);
    }

    [HttpGet("geo-data")]
    public async Task<IActionResult> GetGeoData([FromQuery] GetRacesFilterInput criteria)
    {
        var result = await _geoDataService.Search(criteria);

        return Ok(result);
    }
}
