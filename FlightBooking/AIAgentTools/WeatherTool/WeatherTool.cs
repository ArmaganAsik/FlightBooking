using FlightBooking.DTOs.AIAgentDTOs;
using FlightBooking.DTOs.WeatherDTOs;
using System.Text.Json;

namespace FlightBooking.AIAgentTools.WeatherTool
{
    public class WeatherTool : IWeatherTool
    {
        private readonly HttpClient _httpClient;

        private const string RapidApiKey = "API_KEY_HERE";
        private const string RapidApiHost = "yahoo-weather5.p.rapidapi.com";
        private const string BaseUrl =
            "https://yahoo-weather5.p.rapidapi.com/weather";

        public WeatherTool(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherResultDto> GetWeatherAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException(
                    "Şehir bilgisi boş bırakılamaz.",
                    nameof(city));
            }

            string encodedCity = Uri.EscapeDataString(city);

            string requestUrl =
                $"{BaseUrl}?location={encodedCity}&format=json&u=c";

            using HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUrl)
            };

            request.Headers.Add("x-rapidapi-key", RapidApiKey);
            request.Headers.Add("x-rapidapi-host", RapidApiHost);

            using HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            string responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Hava durumu API isteği başarısız oldu. " +
                    $"Durum kodu: {(int)response.StatusCode}. " +
                    $"Cevap: {responseContent}");
            }

            using JsonDocument document =
                JsonDocument.Parse(responseContent);

            JsonElement root = document.RootElement;

            JsonElement location =
                root.GetProperty("location");

            JsonElement currentObservation =
                root.GetProperty("current_observation");

            JsonElement wind =
                currentObservation.GetProperty("wind");

            JsonElement atmosphere =
                currentObservation.GetProperty("atmosphere");

            JsonElement astronomy =
                currentObservation.GetProperty("astronomy");

            JsonElement condition =
                currentObservation.GetProperty("condition");

            WeatherResultDto weatherResult = new WeatherResultDto
            {
                City = location
                    .GetProperty("city")
                    .GetString() ?? city,

                Country = location
                    .GetProperty("country")
                    .GetString() ?? string.Empty,

                TimeZoneId = location
                    .GetProperty("timezone_id")
                    .GetString() ?? string.Empty,

                Temperature = condition
                    .GetProperty("temperature")
                    .GetDecimal(),

                Condition = condition
                    .GetProperty("text")
                    .GetString() ?? "Bilinmiyor",

                Humidity = atmosphere
                    .GetProperty("humidity")
                    .GetInt32(),

                WindSpeed = wind
                    .GetProperty("speed")
                    .GetDouble(),

                WindDirection = wind
                    .GetProperty("direction")
                    .GetString() ?? string.Empty,

                Visibility = atmosphere
                    .GetProperty("visibility")
                    .GetInt32(),

                Pressure = atmosphere
                    .GetProperty("pressure")
                    .GetInt32(),

                Sunrise = astronomy
                    .GetProperty("sunrise")
                    .GetString() ?? string.Empty,

                Sunset = astronomy
                    .GetProperty("sunset")
                    .GetString() ?? string.Empty
            };

            if (root.TryGetProperty(
                    "forecasts",
                    out JsonElement forecastsElement))
            {
                foreach (JsonElement forecast in
                         forecastsElement.EnumerateArray())
                {
                    weatherResult.Forecasts.Add(
                        new WeatherForecastResultDto
                        {
                            Day = forecast
                                .GetProperty("day")
                                .GetString() ?? string.Empty,

                            Date = forecast
                                .GetProperty("date")
                                .GetInt64(),

                            Low = forecast
                                .GetProperty("low")
                                .GetInt32(),

                            High = forecast
                                .GetProperty("high")
                                .GetInt32(),

                            Condition = forecast
                                .GetProperty("text")
                                .GetString() ?? string.Empty
                        });
                }
            }

            return weatherResult;
        }
    }
}