using FlightBooking.DTOs.FlightSearchDTOs;
using FlightBooking.Models;
using FlightBooking.Services.AirportServices;
using FlightBooking.Services.FlightSearchServices;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace FlightBooking.Controllers
{
    public class FlightController : Controller
    {
        private readonly IAirPortService _airportService;
        private readonly IFlightSearchService _flightSearchService;

        public FlightController(IAirPortService airportService, IFlightSearchService flightSearchService)
        {
            _airportService = airportService;
            _flightSearchService = flightSearchService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchAirport(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Json(new List<object>());

            try
            {
                List<AirportResult> airports = await _airportService.SearchAirportsAsync(query);
                return Json(airports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResolveIata(string fromCity, string toCity)
        {
            AirportResult from = await _airportService.GetFirstIataAsync(fromCity);
            AirportResult to = await _airportService.GetFirstIataAsync(toCity);

            return Json(new
            {
                fromIata = from?.Iata,
                fromAirpot = from?.AirportName,
                toIata = to?.Iata,
                toAirport = to?.AirportName
            });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchFlights(string fromCity, string toCity, string depart, int adults = 1, string cabin = "economy", string currency = "TRY")
        {
            FlightSearchResultDto result = new FlightSearchResultDto { Currency = currency };

            try
            {
                // 1) Şehir isimlerinden IATA çöz
                AirportResult from = await _airportService.GetFirstIataAsync(fromCity);
                AirportResult to = await _airportService.GetFirstIataAsync(toCity);

                if (from?.Iata == null || to?.Iata == null)
                {
                    result.Success = false;
                    result.Error = "Kalkış veya varış için havalimanı bulunamadı.";
                    return Json(result);
                }

                result.FromIata = from.Iata;
                result.FromAirport = from.AirportName;
                result.ToIata = to.Iata;
                result.ToAirport = to.AirportName;

                // 2) IATA'larla uçuşları getir
                result.Flights = await _flightSearchService.SearchAsync(
                    from.Iata, to.Iata, depart, adults, cabin, currency);

                result.Success = true;
                return Json(result);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                return Json(result);
            }
        }
    }
}
