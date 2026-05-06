using HealthApp.Business.DTOs;
using HealthApp.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("Token içinde kullanıcı kimliği bulunamadı.");
            return Guid.Parse(userId);
        }

        // GET /api/User/profile — Flutter: GET /api/users/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(GetUserId());
                if (user == null) return NotFound(new { message = "Kullanıcı bulunamadı." });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/User/profile — Flutter: PUT /api/users/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
        {
            try
            {
                var updated = await _userService.UpdateUserAsync(GetUserId(), dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserCreateDTO userCreateDto)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(userCreateDto);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}