using FlightBooking.DTOs.AIAgentDTOs;

namespace FlightBooking.AIAgentServices.OpenAIServices
{
    public interface IOpenAIService
    {
        Task<AgentResponseDto> GetResponseAsync(string prompt);
    }
}
