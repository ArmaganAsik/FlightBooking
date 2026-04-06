using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.DTOs.PassengerDTOs;
using FlightBooking.Services.FlightServices;
using FlightBooking.ViewModels.PassengerVMs;
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
            await _flightService.CreateFlightAsync(createFlightDto);
            return RedirectToAction("ListFlights");
        }

        public async Task<IActionResult> GetFlightPassengers(string id)
        {
            GetFlightByIdDto flight = await _flightService.GetFlightByIdAsync(id);
            List<PassengerListItemDto> passengers = await _flightService.GetFlightPassengersAsync(id);
            FlightPassengersVm flightPassengersVm = new FlightPassengersVm
            {
                Flight = flight,
                Passengers = passengers
            };
            return View(flightPassengersVm);
        }
    }
}
