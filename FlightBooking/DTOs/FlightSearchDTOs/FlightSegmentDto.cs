namespace FlightBooking.DTOs.FlightSearchDTOs
{
    // Her uçuş bacağı
    public class FlightSegmentDto
    {
        public string Airline { get; set; } = "";
        public string AirlineLogo { get; set; } = "";
        public string FlightNumber { get; set; } = "";
        public string Aircraft { get; set; } = "";
        public string Legroom { get; set; } = "";       // "76 cm"
        public string DurationText { get; set; } = "";   // "1 hr 5 min"

        public string DepartureTime { get; set; } = "";  // ham: "2026-8-25 07:30"
        public string DepartureCode { get; set; } = "";  // "ESB"
        public string DepartureName { get; set; } = "";  // "Ankara Esenboga Airport"

        public string ArrivalTime { get; set; } = "";
        public string ArrivalCode { get; set; } = "";
        public string ArrivalName { get; set; } = "";
    }
}