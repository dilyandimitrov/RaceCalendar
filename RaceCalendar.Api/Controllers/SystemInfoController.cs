using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Domain.Queries;

namespace RaceCalendar.Api.Controllers;

[Route("v1/api/[Controller]")]
[ApiController]
public class SystemInfoController : ControllerBase
{
    private readonly IGetSystemInfoQuery _getSystemInfoQuery;

    public SystemInfoController(IGetSystemInfoQuery getSystemInfoQuery)
    {
        _getSystemInfoQuery = getSystemInfoQuery ?? throw new ArgumentNullException(nameof(getSystemInfoQuery));
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {
        var systemInfo = await _getSystemInfoQuery.Get();

        return Ok(systemInfo);
    }
}
