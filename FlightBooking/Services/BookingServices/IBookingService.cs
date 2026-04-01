using FlightBooking.DTOs.BookingDTOs;

namespace FlightBooking.Services.BookingServices
{
    public interface IBookingService
    {
        Task CreateBooking(CreateBookingDto createBookingDto);
    }
}
