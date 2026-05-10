using HealthApp.Business.DTOs.Gemini;
using HealthApp.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public ChatController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] GeminiChatRequestDTO request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { Message = "Lutfen gecerli bir metin girin." });
            }

            var result = await _geminiService.ProcessMessageAsync(request.Message);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(result.Data);
        }
    }
}
