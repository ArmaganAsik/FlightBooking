using Amazon.Runtime;
using FlightBooking.Models;
using System.Text.Json;

namespace FlightBooking.Services.AirportServices
{
    public class AirportService : IAirPortService
    {
        private readonly HttpClient _client;

        public AirportService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<AirportResult>> SearchAirportsAsync(string query)
        {
            List<AirportResult> results = new List<AirportResult>();

            if (string.IsNullOrWhiteSpace(query)) return results;

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(
                    $"https://google-flights2.p.rapidapi.com/api/v1/searchAirport" +
                    $"?query={Uri.EscapeDataString(query)}&language_code=en-US&country_code=US"),
                Headers =
                {
                    { "x-rapidapi-key", "API_KEY_HERE" },
                    { "x-rapidapi-host", "google-flights2.p.rapidapi.com" },
                }
            };

            using HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();

            AirportSearchResponse parsed = JsonSerializer.Deserialize<AirportSearchResponse>(body);

            if (parsed?.Data == null) return results;

            foreach (AirportSearchData data in parsed.Data)
            {
                foreach (AirportItem airport in data.List)
                {
                    if (airport.Type == "airport" && !string.IsNullOrWhiteSpace(airport.Id))
                    {
                        results.Add(new AirportResult
                        {
                            Iata = airport.Id,
                            AirportName = airport.Title ?? "",
                            City = airport.City ?? data.City ?? "",
                            Title = data.Title ?? ""
                        });
                    }
                }
            }

            return results;
        }

        public async Task<AirportResult?> GetFirstIataAsync(string query)
        {
            List<AirportResult> list = await SearchAirportsAsync(query);
            return list.FirstOrDefault();
        }
    }
}
