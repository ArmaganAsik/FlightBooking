using FlightBooking.DTOs.RestaurantDTOS;
using System.Text;
using System.Text.Json;

namespace FlightBooking.AIAgentServices.GooglePlacesServices
{
    public class GooglePlacesService : IGooglePlacesService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GooglePlacesService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<List<RestaurantResultDto>> SearchRestaurantAsync(string query)
        {
            string apiKey = _configuration["GoogleMaps:ApiKey"];

            var request = new
            {
                textQuery = query,
                languageCode = "tr",
                regionCode = "TR",
                maxResultCount = 5
            };

            StringContent content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", "places.id,places.displayName,places.formattedAddress,places.rating,places.userRatingCount,places.googleMapsUri");

            HttpResponseMessage response = await _httpClient.PostAsync("https://places.googleapis.com/v1/places:searchText", content);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            // Burayı bir sonraki adımda deserialize edeceğiz.
            return new List<RestaurantResultDto>();
        }
    }
}
