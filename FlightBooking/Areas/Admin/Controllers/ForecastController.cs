using FlightBooking.MachineLearningModels;
using FlightBooking.Services.MachineLearningServices.FlightDataServices;
using FlightBooking.Services.MachineLearningServices.NoShowDataServices;
using FlightBooking.Services.MachineLearningServices.Prediction;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ForecastController : Controller
    {
        private readonly IFlightDataService _mongoFlightDataService;
        private readonly IFlightMLService _flightMLService;
        private readonly INoShowService _noShowService;

        public ForecastController(IFlightDataService mongoFlightDataService, IFlightMLService flightMLService, INoShowService noShowService)
        {
            _mongoFlightDataService = mongoFlightDataService;
            _flightMLService = flightMLService;
            _noShowService = noShowService;
        }

        public async Task<IActionResult> TrainModel()
        {
            List<FlightData> mldata = await _mongoFlightDataService.ConvertToMlDataAsync();
            _flightMLService.Train(mldata);
            ViewBag.Message = "Model başarıyla eğitildi.";
            return View();
        }

        public IActionResult Predict()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Predict(DateTime flightDate, string flightType)
        {
            FlightData input = new FlightData
            {
                Month = flightDate.Month,

                DayOfWeek = (float)flightDate.DayOfWeek,

                FlightType = flightType == "Morning" ? 0 : 1
            };

            FlightPrediction prediction = _flightMLService.Predict(input);

            ViewBag.Result = prediction.PredictedLabel
                ? "Bu uçuş büyük ihtimal dolacaktır."
                : "Bu uçuşta yoğunluk düşük görünüyor.";

            ViewBag.Probability = prediction.Probability;

            return View();
        }

        public async Task<IActionResult> NoShowAnalysis()
        {
            var values = await _noShowService.GetSlotBasedNoShowRatesAsync();
            return View(values);
        }
    }
}