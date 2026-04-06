using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.DTOs.PassengerDTOs;

namespace FlightBooking.ViewModels.PassengerVMs
{
    public class FlightPassengersVm
    {
        public GetFlightByIdDto Flight { get; set; }
        public List<PassengerListItemDto> Passengers { get; set; }
    }
}
