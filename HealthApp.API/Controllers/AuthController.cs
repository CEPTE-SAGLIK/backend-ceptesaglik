using HealthApp.Business.DTOs;
using HealthApp.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthApp.API.Controllers
{
    // api/ prefix'i tüm controller'larda tutarlılık sağlar.
    // Endpoint: /api/Auth/login, /api/Auth/register
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        // Dependency Injection: ASP.NET, Program.cs'de kayıtlı olan AuthService'i otomatik verir.
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST /auth/register
        // Flutter'daki AuthRepository.register() buraya istek atar.
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO registerDto)
        {
            try
            {
                var user = await _authService.RegisterAsync(registerDto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/Auth/login
        // Flutter'daki AuthRepository.login() buraya istek atar.
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginDto)
        {
            try
            {
                var user = await _authService.LoginAsync(loginDto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/Auth/refresh
        // Flutter access token süresi dolduğunda buraya istek atar.
        // Eski refresh token gönderilir → Yeni access + refresh token döner.
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDTO refreshDto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
