using FlightBooking.DTOs.NoShowDTOs;
using FlightBooking.MachineLearningModels;
using FlightBooking.Services.MachineLearningServices.NoShowDataServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OverbookingController : Controller
    {
        private readonly INoShowService _noShowService;

        private readonly IOverbookingRecommendationService _overbookingRecommendationService;

        public OverbookingController(INoShowService noShowService, IOverbookingRecommendationService overbookingRecommendationService)
        {
            _noShowService = noShowService;
            _overbookingRecommendationService = overbookingRecommendationService;
        }

        public async Task<IActionResult> Index()
        {
            List<NoShowHistory> flights =
                await _noShowService
                    .GetAllAsync();

            List<OverbookingRecommendationResultDto> recommendations =
                new List<OverbookingRecommendationResultDto>();

            foreach (var flight in flights)
            {
                OverbookingRecommendationResultDto recommendation =
                    await _overbookingRecommendationService
                        .GenerateRecommendationAsync(
                            flightDate:
                                flight.FlightDate,

                            flightSlot:
                                flight.FlightSlot,

                            passengerCount:
                                flight.BoardedPassenger,

                            capacity:
                                flight.Capacity);

                recommendations.Add(recommendation);
            }

            OverbookingDashboardDto dto = new OverbookingDashboardDto
            {
                Recommendations = recommendations,

                AverageNoShowRate =
                    recommendations
                        .Average(
                            x => x.ExpectedNoShowRate),

                MostRiskySlot =
                    recommendations
                        .OrderByDescending(
                            x => x.ExpectedNoShowRate)
                        .First()
                        .FlightSlot,

                MostStableSlot =
                    recommendations
                        .OrderBy(
                            x => x.ExpectedNoShowRate)
                        .First()
                        .FlightSlot,

                SuggestedOverbookingRate =
                    recommendations
                        .Average(
                            x => x.ExtraSellableSeatCount),

                TotalFlightCount =
                    recommendations.Count,

                TotalPassengerCount =
                    recommendations.Sum(
                        x => x.ActualPassengerCount),

                AiInsights = new List<string>
                {
                    "Weekend evening flights show higher no-show tendency.",

                    "Morning-1 suitable for controlled overbooking.",

                    "Evening-2 requires standby crew planning."
                }
            };

            return View(dto);
        }

        public async Task<IActionResult> PredictionJan27()
        {
            List<OverbookingForecastResultDto> values = await _overbookingRecommendationService.PredictJanuary2027Async();
            return View(values);
        }
    }
}
