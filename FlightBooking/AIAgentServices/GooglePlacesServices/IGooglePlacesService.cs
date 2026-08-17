using FlightBooking.DTOs.RestaurantDTOS;

namespace FlightBooking.AIAgentServices.GooglePlacesServices
{
    public interface IGooglePlacesService
    {
        Task<List<RestaurantResultDto>> SearchRestaurantAsync(string query);
    }
}
