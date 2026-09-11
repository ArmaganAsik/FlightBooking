using System.Text.Json.Serialization;

namespace FlightBooking.Models.FlightSearchModels
{
    public class FlightApiDuration
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
