using FlightBooking.DTOs.AIAgentDTOs;

namespace FlightBooking.AIAgentTools.WeatherTool
{
    public class WeatherTool : IWeatherTool
    {
        public async Task<WeatherResultDto> GetWeatherAsync(string city)
        {
            return await Task.FromResult(new WeatherResultDto
            {
                City = city,
                Temperature = 24,
                Condition = "Güneşli",
                Humidity = 58,
                WindSpeed = 11,
                Advice = "Güneş gözlüğü almanız önerilir."
            });
        }
    }
}
