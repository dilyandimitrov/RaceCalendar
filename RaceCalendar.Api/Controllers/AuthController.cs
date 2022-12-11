using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Domain.Models.Authentication;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Api.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.RegisterUserAsync(request);

            if (result.IsSendConfirmationSucceeded)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        return BadRequest(new RegistrationResponse()
        {
            Result = false,
            Errors = new List<string>() { "Невалидна заявка за регистриране на потребител." }
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.LoginAsync(request);

            if (result.IsEmailConfirmed == false)
            {
                return BadRequest(result);
            }

            if (!string.IsNullOrEmpty(result.Token) && result.ExpiresAt.HasValue)
            {
                return Ok(result);
            }

        }

        return BadRequest(new RegistrationResponse()
        {
            Errors = new List<string>() { "Грешен email или парола." }
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("confirm-email")]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        if (ModelState.IsValid)
        {
            var isSuccessful = await _userService.ConfirmEmail(request);

            if (isSuccessful)
            {
                return Ok();
            }
        }

        return BadRequest();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("send-email-confirmation")]
    public async Task<ActionResult> SendEmailConfirmation([FromBody] SendEmailConfirmationRequest request)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.SendEmailConfirmation(request);

            if (result.IsSendConfirmationSucceeded)
            {
                return Ok(result);
            }
        }

        return BadRequest(new RegistrationResponse());
    }
}
