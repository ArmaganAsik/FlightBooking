using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.FlightServices;
using FlightBooking.ViewModels.CheckInVMs;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CheckInController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public CheckInController(IFlightService flightService, IBookingService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(string? passengerId)
        {
            GetBookingByIdDto booking = await _bookingService.GetBookingByPassengerIdAsync(passengerId);
            GetFlightByIdDto flight = await _flightService.GetFlightByIdAsync(booking.FlightId);
            CreateCheckInVm createCheckInVm = new CreateCheckInVm
            {
                Flight = flight
            };
            return View(createCheckInVm);
        }
    }
}
