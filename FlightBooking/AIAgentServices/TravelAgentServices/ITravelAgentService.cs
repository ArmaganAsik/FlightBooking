using FlightBooking.DTOs.AIAgentDTOs;

namespace FlightBooking.AIAgentServices.TravelAgentServices
{
    public interface ITravelAgentService
    {
        Task<AgentResponseDto> AskAgentAsync(string prompt);
    }
}
