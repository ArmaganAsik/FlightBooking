using System.Text.Json.Serialization;

namespace FlightBooking.Models.FlightSearchModels
{
    public class FlightApiBags
    {
        [JsonPropertyName("carry_on")]
        public int? CarryOn { get; set; }

        [JsonPropertyName("checked")]
        public int? Checked { get; set; }
    }
}
