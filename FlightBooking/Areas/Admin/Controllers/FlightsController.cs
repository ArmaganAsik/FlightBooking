using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.DTOs.PassengerDTOs;
using FlightBooking.Services.FlightServices;
using FlightBooking.ViewModels.FlightVMs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FlightsController : Controller
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        public async Task<IActionResult> ListFlights()
        {
            List<ResultFlightDto> values = await _flightService.GetAllFlightsAsync();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateFlight()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlight(CreateFlightDto createFlightDto)
        {
            createFlightDto.BoardingTime = createFlightDto.DepartureTime.Date.Add(createFlightDto.BoardingTime.Value.TimeOfDay);
            await _flightService.CreateFlightAsync(createFlightDto);
            return RedirectToAction("ListFlights");
        }

        public async Task<IActionResult> GetFlightBookings(string id)
        {
            GetFlightByIdDto flight = await _flightService.GetFlightByIdAsync(id);
            List<ResultBookingDto> bookings = await _flightService.GetFlightBookingsAsync(id);
            FlightBookingsVm flightBookingsVm = new FlightBookingsVm
            {
                Flight = flight,
                Bookings = bookings
            };
            return View(flightBookingsVm);
        }
    }
}
