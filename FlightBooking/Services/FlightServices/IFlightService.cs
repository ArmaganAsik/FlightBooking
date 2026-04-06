using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.DTOs.PassengerDTOs;

namespace FlightBooking.Services.FlightServices
{
    public interface IFlightService
    {
        Task<List<ResultFlightDto>> GetAllFlightsAsync();
        Task<GetFlightByIdDto> GetFlightByIdAsync(string id);
        Task CreateFlightAsync(CreateFlightDto createFlightDto);
        Task DeleteFlightAsync(string id);
        Task UpdateFlightAsync(UpdateFlightDto updateFlightDto);
        Task<List<PassengerListItemDto>> GetFlightPassengersAsync(string id);
    }
}
