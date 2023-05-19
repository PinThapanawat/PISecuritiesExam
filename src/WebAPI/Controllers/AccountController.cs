using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.DTOs;
using WebAPI.Services.Interferes;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController: ControllerBase
{
    private readonly IIdentityService _identityService;

    public AccountController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var result = await _identityService.LoginAsync(model);
        if (!string.IsNullOrEmpty(result?.Token))
        {
            return Ok(result);
        }
        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        var result = await _identityService.RegisterAsync(model);
        return Ok(result);
    }

}