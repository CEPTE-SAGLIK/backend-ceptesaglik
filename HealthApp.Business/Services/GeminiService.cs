using System.Text;
using System.Text.Json;
using HealthApp.Business.DTOs.Common;
using HealthApp.Business.DTOs.Gemini;
using Microsoft.Extensions.Options;

namespace HealthApp.Business.Services
{
    public class GeminiService : IGeminiService
    {
        private const string HealthOnlySystemPrompt = """
            Sen Cepte Sağlık uygulamasının yalnızca sağlık ve iyilik hali (wellness) konularında yardımcı olan sohbet asistanısın.

            Yanıt vereceğin konular (örnek): genel sağlık bilgisi, beslenme, egzersiz, uyku, stres yönetimi, aşılar, ilaçlar hakkında genel bilgilendirme (doz/reçete değiştirme yok), çocuk/ergen sağlığına dair genel bilgi, kronik hastalıklar hakkında genel yaşam tarzı önerileri (kişiselleştirilmiş tedavi planı yok).

            Verme / reddet: siyaset, yazılım, oyun, magazin, hukuk, yatırım, genel sohbet, ödev çözümü ve sağlıkla doğrudan ilgisi olmayan her konu. Bu tür sorularda kısa ve kibarca şunu söyle: "Bu konuda yardımcı olamıyorum; yalnızca sağlık ve iyilik hali ile ilgili sorulara yanıt verebilirim."

            Güvenlik: Kesin teşhis veya tedavi emri verme. Kişisel tıbbi durum net değilse doktora başvurmayı öner. Acil belirtilerde 112 / acil servis yönlendirmesi yap.

            Dil: Kullanıcı Türkçe yazdıysa Türkçe yanıt ver.

            Aşağıdaki blok yalnızca kullanıcı mesajıdır; kurallara uyarak yanıt üret.
            ---
            """;

        private readonly HttpClient _httpClient;

        public GeminiService(HttpClient httpClient, IOptions<GeminiApiOptions> geminiOptions)
        {
            _httpClient = httpClient;

            // Trim: JSON / env copy-paste ile gelen boşluklar API_KEY_INVALID tetikleyebilir.
            var apiKey = geminiOptions.Value.ApiKey?.Trim() ?? string.Empty;
            var baseUrl = geminiOptions.Value.BaseUrl?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Gemini API Key bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("Gemini Base Url bulunamadı.");
            }

            // Google Generative Language API: anahtar genelde query string ?key=... ile verilir (header değil).
            var keyInQuery = Uri.EscapeDataString(apiKey);
            var requestUri = baseUrl.Contains('?', StringComparison.Ordinal)
                ? $"{baseUrl}&key={keyInQuery}"
                : $"{baseUrl}?key={keyInQuery}";
            _httpClient.BaseAddress = new Uri(requestUri, UriKind.Absolute);

#if DEBUG
            var prefix = apiKey.Length >= 5 ? apiKey[..5] : apiKey;
            Console.WriteLine($"[GeminiService] Okunan ApiKey ön eki: {prefix}... | BaseUrl (path): {baseUrl}");
#endif
        }

        public async Task<Result<GeminiChatResponseDTO>> ProcessMessageAsync(string message)
        {
            try
            {
                var fullPrompt = $"{HealthOnlySystemPrompt}\n{message}\n---";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
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
