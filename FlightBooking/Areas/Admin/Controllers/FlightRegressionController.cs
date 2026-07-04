using FlightBooking.MachineLearningRegressionModels;
using FlightBooking.Services.MachineLearningServices.FlightDataServices;
using FlightBooking.Services.MachineLearningServices.Prediction;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FlightRegressionController : Controller
    {
        private readonly IFlightRegressionService _flightRegressionService;
        private readonly IFlightDataService _mongoFlightDataService;
        private readonly IFlightMLService _flightMLService;

        public FlightRegressionController(IFlightRegressionService flightRegressionService, IFlightDataService mongoFlightDataService, IFlightMLService flightMLService)
        {
            _flightRegressionService = flightRegressionService;
            _mongoFlightDataService = mongoFlightDataService;
            _flightMLService = flightMLService;
        }

        public async Task<IActionResult> TrainRegressionModel()
        {
            List<FlightRegressionData> regressionData = await _mongoFlightDataService.ConvertToRegressionDataAsync();
            _flightRegressionService.Train(regressionData);
            ViewBag.Message = "Regression modeli başarıyla eğitildi.";
            return View();
        }

        public IActionResult January2027Forecast()
        {
            List<string> result = new List<string>();

            for (int day = 1; day <= 31; day++)
            {
                DateTime date = new DateTime(2027, 1, day);

                // 🌅 Morning
                FlightRegressionData morningInput = new FlightRegressionData
                {
                    Month = date.Month,
                    DayOfWeek = (float)date.DayOfWeek,
                    FlightType = 0
                };

                FlightRegressionPrediction morningPrediction = _flightRegressionService.Predict(morningInput);

                // 🌙 Evening
                FlightRegressionData eveningInput = new FlightRegressionData
                {
                    Month = date.Month,
                    DayOfWeek = (float)date.DayOfWeek,
                    FlightType = 1
                };

                FlightRegressionPrediction eveningPrediction = _flightRegressionService.Predict(eveningInput);

                result.Add(
                    $"{date:dd.MM.yyyy} → Morning: {morningPrediction.Score:0} yolcu | Evening: {eveningPrediction.Score:0} yolcu");
            }

            return View(result);
        }
    }
}
