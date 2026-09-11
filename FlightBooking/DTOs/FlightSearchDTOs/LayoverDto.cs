namespace FlightBooking.DTOs.FlightSearchDTOs
{
    // Aktarma (bekleme)
    public class LayoverDto
    {
        public string AirportCode { get; set; } = "";
        public string AirportName { get; set; } = "";
        public string City { get; set; } = "";
        public string DurationLabel { get; set; } = "";  // "1 hr 5 min"
    }
}