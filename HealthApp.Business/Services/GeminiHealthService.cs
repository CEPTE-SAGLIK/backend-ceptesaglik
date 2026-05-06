using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HealthApp.Business.DTOs.Common;
using HealthApp.Business.DTOs.Gemini;
using Microsoft.Extensions.Configuration;

namespace HealthApp.Business.Services
{
    public class GeminiHealthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiHealthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // appsettings'ten Gemini configlerini al
            _apiKey = configuration["GeminiApi:ApiKey"] ?? throw new Exception("Gemini API Key bulunamadı.");
            var baseUrl = configuration["GeminiApi:BaseUrl"] ?? throw new Exception("Gemini Base Url bulunamadı.");
            
            _httpClient.BaseAddress = new Uri($"{baseUrl}?key={_apiKey}");
        }

        public async Task<Result<GeminiAnalysisResponseDTO>> AnalyzeHealthTextAsync(string text)
        {
            try
            {
                // Prompta JSON formatında dönüş zorlayan mühendislik ekleyelim
                var prompt = $@"
Aşağıdaki metinden varsa ilaç kullanım ve hatırlatıcı/randevu bilgilerini çıkar.
Çıktıyı SADECE ve kesinlikle aşağıdaki JSON formatında ver. Başında veya sonunda markdown işaretleri (```json vb.) OLMASIN.
Eğer bir bilgi yoksa listeyi boş bırak.

Format:
{{
  ""medicines"": [
    {{
      ""name"": ""string"",
      ""frequencyType"": ""Daily|Weekly|EveryOtherDay|Custom"",
      ""timesPerDay"": int,
      ""usageInstructions"": ""string veya null""
    }}
  ],
  ""reminders"": [
    {{
      ""title"": ""string"",
      ""type"": ""Medicine|Vaccine|Appointment|Custom"",
      ""dateTime"": ""YYYY-MM-DDTHH:mm:ss""
    }}
  ]
}}

Metin: ""{text}""
";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Result<GeminiAnalysisResponseDTO>.Failure($"Gemini API error: {error}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                
                // Gemini dönen yapısı
                using var document = JsonDocument.Parse(responseString);
                var candidates = document.RootElement.GetProperty("candidates");
                if (candidates.GetArrayLength() == 0)
                {
                    return Result<GeminiAnalysisResponseDTO>.Failure("Gemini boş cevap döndü.");
                }

                var geminiText = candidates[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                // Gemini bazen backtick ile (```json ... ```) dönüyor, temizleyelim
                if (geminiText!.StartsWith("```json"))
                {
                    geminiText = geminiText.Replace("```json", "").Replace("```", "").Trim();
                }
                
                if (geminiText.StartsWith("```"))
                {
                    geminiText = geminiText.Replace("```", "").Trim();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var resultDto = JsonSerializer.Deserialize<GeminiAnalysisResponseDTO>(geminiText, options);

                if (resultDto == null)
                {
                    return Result<GeminiAnalysisResponseDTO>.Failure("JSON deserialize edilemedi.");
                }

                return Result<GeminiAnalysisResponseDTO>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return Result<GeminiAnalysisResponseDTO>.Failure($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
