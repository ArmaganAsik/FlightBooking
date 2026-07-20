using FlightBooking.DTOs.MachineLearningOverbookingDTOs;
using FlightBooking.DTOs.NoShowDTOs;
using FlightBooking.MachineLearningModels;
using FlightBooking.Settings;
using Microsoft.ML;
using MongoDB.Driver;

namespace FlightBooking.Services.MachineLearningServices.NoShowDataServices
{
    public class OverbookingRecommendationService : IOverbookingRecommendationService
    {
        private readonly INoShowService _noShowService;
        private readonly IMongoCollection<NoShowHistory> _noShowCollection;
        private readonly MLContext _mlContext;

        public OverbookingRecommendationService(INoShowService noShowService, IDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            _noShowCollection = database.GetCollection<NoShowHistory>(settings.NoShowHistoryCollection);
            _mlContext = new MLContext();
            _noShowService = noShowService;
        }

        public async Task<OverbookingRecommendationResultDto> GenerateRecommendationAsync(string flightDate, string flightSlot, int passengerCount, int capacity)
        {
            var slotRates = await _noShowService.GetSlotBasedNoShowRatesAsync();
            double noShowRate = 0;

            if (slotRates.ContainsKey(flightSlot))
            {
                noShowRate = slotRates[flightSlot];
            }

            int expectedNoShowPassenger = (int)Math.Round(passengerCount * (noShowRate / 100));
            int recommendedMaxSale = capacity + expectedNoShowPassenger;
            int extraSellableSeat = recommendedMaxSale - capacity;
            string riskLevel = "Low";

            if (noShowRate >= 7)
                riskLevel = "High";
            else if (noShowRate >= 5)
                riskLevel = "Medium";

            string recommendation =
                riskLevel switch
                {
                    "High" =>
                        "Agresif overbooking uygulanabilir",

                    "Medium" =>
                        "Kontrollü overbooking önerilir",

                    _ =>
                        "Standart satış politikası önerilir"
                };

            return new OverbookingRecommendationResultDto
            {
                FlightDate = flightDate,
                FlightSlot = flightSlot,
                ActualPassengerCount = passengerCount,
                Capacity = capacity,
                ExpectedNoShowRate = noShowRate,
                ExpectedNoShowPassenger = expectedNoShowPassenger,
                RecommendedMaxTicketSale = recommendedMaxSale,
                ExtraSellableSeatCount = extraSellableSeat,
                RiskLevel = riskLevel,
                Recommendation = recommendation
            };
        }

        public async Task<List<OverbookingForecastResultDto>> PredictJanuary2027Async()
        {
            List<NoShowHistory> historicalData = await _noShowCollection.Find(_ => true).ToListAsync();

            // ML Training Data
            List<NoShowPredictionDataDto> trainingData = historicalData.Select(x => new NoShowPredictionDataDto
            {
                Month = DateTime.Parse(x.FlightDate).Month,
                DayOfWeek = (float)DateTime.Parse(x.FlightDate).DayOfWeek,
                FlightSlot = ConvertFlightSlotToNumber(x.FlightSlot),
                Capacity = x.Capacity,
                SoldTickets = x.SoldTickets,
                OnlineCheckedIn = x.OnlineCheckedIn,
                AirportCheckedIn = x.AirportCheckedIn,
                MissedConnection = x.MissedConnection,
                CancelledPassenger = x.CancelledPassenger,
                NoShowPassenger = x.NoShowPassenger
            }).ToList();

            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Concatenate(
                        "Features",
                        nameof(NoShowPredictionDataDto.Month),
                        nameof(NoShowPredictionDataDto.DayOfWeek),
                        nameof(NoShowPredictionDataDto.FlightSlot),
                        nameof(NoShowPredictionDataDto.Capacity),
                        nameof(NoShowPredictionDataDto.SoldTickets),
                        nameof(NoShowPredictionDataDto.OnlineCheckedIn),
                        nameof(NoShowPredictionDataDto.AirportCheckedIn),
                        nameof(NoShowPredictionDataDto.MissedConnection),
                        nameof(NoShowPredictionDataDto.CancelledPassenger))
                    .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: "NoShowPassenger", featureColumnName: "Features"));

            var model = pipeline.Fit(dataView);

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<NoShowPredictionDataDto, NoShowPredictionResultDto>(model);

            List<OverbookingForecastResultDto> results = new List<OverbookingForecastResultDto>();

            // Gerçek slot template'leri DB’den alınır
            List<NoShowHistory> slotTemplates = historicalData.GroupBy(x => x.FlightSlot).Select(g => g.First()).ToList();

            for (int day = 1; day <= 31; day++)
            {
                DateTime date = new DateTime(2027, 1, day);

                foreach (NoShowHistory slot in slotTemplates)
                {
                    NoShowPredictionDataDto sample = new NoShowPredictionDataDto
                    {
                        Month = 1,
                        DayOfWeek = (float)date.DayOfWeek,
                        FlightSlot = ConvertFlightSlotToNumber(slot.FlightSlot),
                        Capacity = slot.Capacity,
                        // Simüle edilen satış
                        SoldTickets = slot.Capacity,
                        // Ortalama check-in davranışı
                        OnlineCheckedIn = slot.Capacity * 0.70f,
                        AirportCheckedIn = slot.Capacity * 0.20f,
                        MissedConnection = 2,
                        CancelledPassenger = 1
                    };

                    NoShowPredictionResultDto prediction = predictionEngine.Predict(sample);
                    int predictedNoShow = (int)Math.Round(prediction.Score);

                    // Negatif prediction koruması
                    if (predictedNoShow < 0)
                        predictedNoShow = 0;

                    int recommendedMaxSale = slot.Capacity + predictedNoShow;

                    string riskLevel = predictedNoShow >= 15 ? "High" : predictedNoShow >= 10 ? "Medium" : "Low";

                    int estimatedRevenue = predictedNoShow * 120;

                    results.Add(
                        new OverbookingForecastResultDto
                        {
                            FlightDate = date.ToString("dd.MM.yyyy"),
                            FlightSlot = slot.FlightSlot,
                            AircraftType = slot.AircraftType,
                            Capacity = slot.Capacity,
                            PredictedNoShow = predictedNoShow,
                            RecommendedMaxSale = recommendedMaxSale,
                            ExtraSeatCount = predictedNoShow,
                            RiskLevel = riskLevel,
                            EstimatedRevenue = estimatedRevenue
                        });
                }
            }

            return results;
        }

        public float ConvertFlightSlotToNumber(string slot)
        {
            return slot switch
            {
                "Morning-1" => 1,
                "Morning-2" => 2,
                "Evening-1" => 3,
                "Evening-2" => 4,
                _ => 0
            };
        }

    }
}
