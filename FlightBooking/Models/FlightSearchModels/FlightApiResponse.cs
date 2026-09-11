using System.Text.Json.Serialization;

namespace FlightBooking.Models.FlightSearchModels
{
    public class FlightApiResponse
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("data")]
        public FlightApiData? Data { get; set; }
    }
}
