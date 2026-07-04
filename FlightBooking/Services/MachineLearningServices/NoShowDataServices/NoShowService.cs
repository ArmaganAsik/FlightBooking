using FlightBooking.MachineLearningModels;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.MachineLearningServices.NoShowDataServices
{
    public class NoShowService : INoShowService
    {
        private readonly IMongoCollection<NoShowHistory> _collection;
        public NoShowService(IDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<NoShowHistory>(settings.NoShowHistoryCollection);
        }

        public async Task<List<NoShowHistory>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Dictionary<string, double>> GetSlotBasedNoShowRatesAsync()
        {
            List<NoShowHistory> data = await GetAllAsync();

            Dictionary<string, double> result = data
                .GroupBy(x => x.FlightSlot).ToDictionary(
                    g => g.Key,
                    g => Math.Round((g.Sum(x => x.NoShowPassenger) * 100.0) / g.Sum(x => x.SoldTickets), 2));

            return result;
        }
    }
}
