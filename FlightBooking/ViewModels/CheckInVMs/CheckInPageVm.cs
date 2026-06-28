using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.DTOs.PassengerDTOs;

namespace FlightBooking.ViewModels.CheckInVMs
{
    public class CheckInPageVm
    {
        public GetFlightByIdDto Flight { get; set; }
        public GetPassengerByIdDto Passenger { get; set; }
        public string PnrNumber { get; set; }
    }
}
