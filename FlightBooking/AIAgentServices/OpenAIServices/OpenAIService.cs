
using FlightBooking.AIAgentSettings;
using FlightBooking.DTOs.AIAgentDTOs;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace FlightBooking.AIAgentServices.OpenAIServices
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAISettings _settings;

        public OpenAIService(HttpClient httpClient, IOptions<OpenAISettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<AgentResponseDto> GetResponseAsync(string prompt)
        {
            var requestBody = new
            {
                model = _settings.Model,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Sen bir seyahat ve restoran öneri asistanısın. Kısa, net ve kullanıcı dostu cevap ver."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                temperature = 0.7
            };

            string json = JsonSerializer.Serialize(requestBody);

            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"
                );

            request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new AgentResponseDto
                {
                    IsSuccess = false,
                    Response = $"OpenAI API hatası: {responseContent}",
                    Model = _settings.Model,
                    ResponseTime = DateTime.Now
                };
            }

            using JsonDocument document = JsonDocument.Parse(responseContent);

            string result = document
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return new AgentResponseDto
            {
                IsSuccess = true,
                Response = result ?? "Cevap alınamadı.",
                Model = _settings.Model,
                ResponseTime = DateTime.Now
            };
        }
    }
}