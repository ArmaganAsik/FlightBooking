using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.FlightDTOs;

namespace FlightBooking.ViewModels.FlightVMs
{
    public class FlightBookingsVm
    {
        public GetFlightByIdDto Flight { get; set; }
        public List<ResultBookingDto> Bookings { get; set; }
    }
}
