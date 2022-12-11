using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Api.Utils;
using RaceCalendar.Domain.Models.Authentication;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Api.Controllers;

[Route("v1/api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.Update(request);

            if (result.Result)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        return BadRequest(new RegistrationResponse()
        {
            Errors = new List<string>() { "Невалидна заявка за промяна на потребител." }
        });
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("delete")]
    public async Task<IActionResult> Delete()
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.Delete(User.GetCurrentUserEmail());

            if (result.Result)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        return BadRequest(new RegistrationResponse()
        {
            Errors = new List<string>() { "Невалидна заявка за промяна на потребител." }
        });
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("get")]
    public async Task<IActionResult> Get()
    {
        var user = await _userService.GetByEmailAsync(User.GetCurrentUserEmail());

        if (user == null)
        {
            return BadRequest();
        }

        return Ok(new UserResponse()
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsAdmin = user.IsAdmin
        });
    }

    [HttpGet]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("getall")]
    public async Task<IActionResult> GeAll()
    {
        return Ok(await _userService.GetAll());
    }
}
