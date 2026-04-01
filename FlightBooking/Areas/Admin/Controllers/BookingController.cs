using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.Entities;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.FlightServices;
using FlightBooking.ViewModels.BookingVMs;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookingController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public BookingController(IFlightService flightService, IBookingService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> CreateBooking(string id)
        {
            GetFlightByIdDto flightByIdDto = await _flightService.GetFlightByIdAsync(id);

            CreateBookingVm createBookingVm = new CreateBookingVm
            {
                Flight = flightByIdDto,
                Booking = new CreateBookingDto
                {
                    FlightId = flightByIdDto.FlightId
                }
            };

            return View(createBookingVm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingVm createBookingVm)
        {
            await _bookingService.CreateBooking(createBookingVm.Booking);
            return RedirectToAction("ListBookings");
        }

        public IActionResult ListBookings()
        {
            return View();
        }
    }
}
