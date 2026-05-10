using System.Text;
using System.Text.Json;
using HealthApp.Business.DTOs.Common;
using HealthApp.Business.DTOs.Gemini;
using Microsoft.Extensions.Configuration;

namespace HealthApp.Business.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var apiKey = configuration["GeminiApi:ApiKey"];
            var baseUrl = configuration["GeminiApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Gemini API Key bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("Gemini Base Url bulunamadı.");
            }

            _httpClient.BaseAddress = new Uri($"{baseUrl}?key={apiKey}");
        }

        public async Task<Result<GeminiChatResponseDTO>> ProcessMessageAsync(string message)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = message }
                            }
                        }
                    }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(string.Empty, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return Result<GeminiChatResponseDTO>.Failure($"Gemini API error: {errorBody}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseString);

                if (!document.RootElement.TryGetProperty("candidates", out var candidates) ||
                    candidates.GetArrayLength() == 0)
                {
                    return Result<GeminiChatResponseDTO>.Failure("Gemini boş cevap döndü.");
                }

                var responseText = candidates[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    return Result<GeminiChatResponseDTO>.Failure("Gemini cevabı okunamadı.");
                }

                return Result<GeminiChatResponseDTO>.Success(new GeminiChatResponseDTO(responseText.Trim()));
            }
            catch (Exception ex)
            {
                return Result<GeminiChatResponseDTO>.Failure($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
