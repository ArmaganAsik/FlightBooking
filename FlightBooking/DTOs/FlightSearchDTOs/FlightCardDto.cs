namespace FlightBooking.DTOs.FlightSearchDTOs
{
    public class FlightCardDto
    {
        public string Airline { get; set; } = "";
        public string AirlineLogo { get; set; } = "";
        public string DepartureTime { get; set; } = "";   // "07:30"
        public string ArrivalTime { get; set; } = "";     // "08:35"
        public string DepartureAirport { get; set; } = ""; // "ESB"
        public string ArrivalAirport { get; set; } = "";   // "COV"
        public string DurationText { get; set; } = "";     // "1 hr 5 min"
        public int Stops { get; set; }                     // aktarma sayısı
        public List<string> LayoverCities { get; set; } = new(); // ["Istanbul"]
        public string Price { get; set; } = "";            // "1410" ya da "unavailable"
        // ---- Detay (modal) ----
        public List<FlightSegmentDto> Segments { get; set; } = new();
        public List<LayoverDto> Layovers { get; set; } = new();
        public BagsDto? Bags { get; set; }
        public CarbonDto? Carbon { get; set; }

    }
}