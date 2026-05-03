using FlightBooking.DTOs.BookingDTOs;

namespace FlightBooking.Services.BookingServices
{
    public interface IBookingService
    {
        Task CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<GetBookingByIdDto> GetBookingByPassengerIdAsync(string passengerId);
    }
}
