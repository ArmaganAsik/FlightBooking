using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.CheckInDTOs;
using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.CheckInServices;
using FlightBooking.Services.FlightServices;
using FlightBooking.ViewModels.CheckInVMs;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CheckInController : Controller
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        public async Task<IActionResult> Index(string? passengerId)
        {
            CheckInDataDto checkInDataDto = await _checkInService.GetCheckInDataAsync(passengerId);

            return View(checkInDataDto);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] CompleteCheckInDto completeCheckInDto)
        {
            await _checkInService.CompleteCheckInAsync(completeCheckInDto);
            return Ok();
            //return RedirectToAction("GetFlightPassengers", "Flights", new { id = completeCheckInDto.FlightId });
        }
    }
}
