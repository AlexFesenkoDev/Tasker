using Microsoft.AspNetCore.Mvc;
using Tasker.Models.Dtos;
using Tasker.Services.Interfaces;

namespace Tasker.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var (isSuccess, error) = await _authService.RegisterAsync(dto);

            if (!isSuccess)
                return BadRequest(error);

            return Ok("Registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            _logger.LogError("Log it manually");

            var (isSuccess, token, error) = await _authService.LoginAsync(dto);

            if (!isSuccess)
                return Unauthorized(error);

            return Ok(new { token });
        }
    }
}
