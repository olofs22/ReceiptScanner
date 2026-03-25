using Microsoft.AspNetCore.Mvc;
using ReceiptProject1.DTOs.AuthDTOs;
using ReceiptProject1.Services;

namespace ReceiptProject1.Controllers;
[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register ([FromBody] RegisterDTO rdto)
    {
        var success = await _authService.Register(rdto);
        if (!success)
            return BadRequest("Email adress already in use.");

        return Ok("User registered successfully!");
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO ldto)
    {
        var token = await _authService.Login(ldto);
        if (token == null)
            return Unauthorized("Invalid email adress or password.");

        return Ok(new { token });
    }
}

