using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.FlightDTOs;

namespace FlightBooking.ViewModels.BookingVMs
{
    public class CreateBookingVm
    {
        public GetFlightByIdDto Flight { get; set; }
        public CreateBookingDto Booking { get; set; }
    }
}
