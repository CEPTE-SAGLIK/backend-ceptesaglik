using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HealthApp.Business.DTOs;
using HealthApp.Business.Services;

namespace HealthApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL: api/notifications
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // 1. Yeni Bildirim Gönderme (POST)
        [HttpPost]
        public async Task<IActionResult> AddNotification([FromBody] NotificationCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _notificationService.AddNotificationAsync(createDto);

            return CreatedAtAction(nameof(AddNotification), new { id = result.Id }, result);
        }

        // 2. Tüm Bildirimleri Listeleme (GET)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _notificationService.GetAllNotificationsAsync();
            return Ok(result);
        }
    }
}