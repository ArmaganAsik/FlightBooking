using FlightBooking.Entities;
using FlightBooking.MachineLearningModels;
using FlightBooking.MachineLearningRegressionModels;
using FlightBooking.Settings;
using MongoDB.Driver;
using NuGet.Configuration;
using System.Collections.Generic;

namespace FlightBooking.Services.MachineLearningServices.FlightDataServices
{
    public class FlightDataService : IFlightDataService
    {
        private readonly IMongoCollection<FlightRawData> _flightRawDataCollection;

        public FlightDataService(IDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            _flightRawDataCollection = database.GetCollection<FlightRawData>(settings.FlightDemandHistoryCollection);
        }

        public async Task<List<FlightRawData>> GetAllAsync()
        {
            return await _flightRawDataCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<FlightData>> ConvertToMlDataAsync()
        {
            List<FlightRawData> rawData = await GetAllAsync();

            List<FlightData> mlData = rawData.Select(x => new FlightData
            {
                Month = DateTime.Parse(x.FlightDate).Month,

                DayOfWeek = (float)DateTime.Parse(x.FlightDate).DayOfWeek,

                FlightType = x.FlightType == "Morning" ? 0 : 1,

                IsFull = x.PassengerCount >= x.Capacity * 0.9
            }).ToList();

            return mlData;
        }

        public async Task<List<FlightRegressionData>> ConvertToRegressionDataAsync()
        {
            List<FlightRawData> rawData = await GetAllAsync();

            List<FlightRegressionData> regressionData = rawData.Select(x => new FlightRegressionData
            {
                Month = DateTime.Parse(x.FlightDate).Month,

                DayOfWeek = (float)DateTime.Parse(x.FlightDate).DayOfWeek,

                FlightType = x.FlightType == "Morning" ? 0 : 1,

                PassengerCount = x.PassengerCount
            }).ToList();

            return regressionData;
        }
    }
}
