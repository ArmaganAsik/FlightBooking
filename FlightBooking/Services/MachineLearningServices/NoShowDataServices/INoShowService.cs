using FlightBooking.MachineLearningModels;

namespace FlightBooking.Services.MachineLearningServices.NoShowDataServices
{
    public interface INoShowService
    {
        Task<List<NoShowHistory>> GetAllAsync();
        Task<Dictionary<string, double>> GetSlotBasedNoShowRatesAsync();
    }
}
