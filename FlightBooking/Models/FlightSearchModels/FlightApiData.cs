using System.Text.Json.Serialization;

namespace FlightBooking.Models.FlightSearchModels
{
    public class FlightApiData
    {
        [JsonPropertyName("itineraries")]
        public FlightApiItineraries? Itineraries { get; set; }
    }
}
