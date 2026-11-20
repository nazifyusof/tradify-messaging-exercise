using Microsoft.AspNetCore.Mvc;
using Tradify.Core;

namespace Tradify.Api.Controllers;

public sealed record LoginRequest(string Username, string Password);

public sealed record LoginResponse(string Id, string Username, string Name, UserRole Role, string CompanyId);

// Exercise stub: any password is accepted for a known username. Not real auth.
[ApiController]
[Route("auth")]
public sealed class AuthController(IUserRepository users) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = string.IsNullOrWhiteSpace(request.Username)
            ? null
            : users.FindByUsername(request.Username.Trim());

        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(new LoginResponse(user.Id, user.Username, user.Name, user.Role, user.CompanyId));
    }
}
