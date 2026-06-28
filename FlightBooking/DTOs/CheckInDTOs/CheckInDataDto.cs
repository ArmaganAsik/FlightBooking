using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.DTOs.PassengerDTOs;

namespace FlightBooking.DTOs.CheckInDTOs
{
    public class CheckInDataDto
    {
        public string PnrNumber { get; set; }
        public GetPassengerByIdDto Passenger { get; set; }
        public GetFlightByIdDto Flight { get; set; }
    }
}
