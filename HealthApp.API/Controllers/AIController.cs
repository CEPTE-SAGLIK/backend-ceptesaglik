using HealthApp.Business.DTOs.Gemini;
using HealthApp.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sadece giriş yapmış kullanıcılar istifade edebilir
    public class AIController : ControllerBase
    {
        private readonly GeminiHealthService _geminiHealthService;

        public AIController(GeminiHealthService geminiHealthService)
        {
            _geminiHealthService = geminiHealthService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeText([FromBody] GeminiAnalyzeRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Lütfen analiz edilecek bir metin girin.");
            }

            var result = await _geminiHealthService.AnalyzeHealthTextAsync(request.Text);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Message = result.Message });
            }

            // Başarılı durumda direkt DTO'yu veya bir wrapper ile dönüyoruz.
            return Ok(result.Data);
        }
    }
}
