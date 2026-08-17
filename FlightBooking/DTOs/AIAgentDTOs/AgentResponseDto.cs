namespace FlightBooking.DTOs.AIAgentDTOs
{
    public class AgentResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Response { get; set; }
        public string Model { get; set; }
        public DateTime ResponseTime { get; set; }
        public string Intent { get; set; }
        public string? City { get; set; }
        public WeatherResultDto? Weather { get; set; }
    }
}
