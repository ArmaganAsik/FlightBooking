using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.CheckInDTOs;
using FlightBooking.DTOs.PassengerDTOs;

namespace FlightBooking.Services.BookingServices
{
    public interface IBookingService
    {
        Task CreateBookingAsync(CreateBookingDto createBookingDto);
    }
}
