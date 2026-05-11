using HealthApp.Business.DTOs.Common;
using HealthApp.Business.DTOs.Gemini;

namespace HealthApp.Business.Services
{
    public interface IGeminiService
    {
        Task<Result<GeminiChatResponseDTO>> ProcessMessageAsync(string message);
    }
}
