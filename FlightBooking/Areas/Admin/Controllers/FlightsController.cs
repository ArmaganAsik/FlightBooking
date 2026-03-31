using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.Services.FlightServices;
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
    }
}
