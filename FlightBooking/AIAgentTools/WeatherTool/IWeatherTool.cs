using FlightBooking.DTOs.AIAgentDTOs;

namespace FlightBooking.AIAgentTools.WeatherTool
{
    public interface IWeatherTool
    {
        Task<WeatherResultDto> GetWeatherAsync(string city);
    }
}
