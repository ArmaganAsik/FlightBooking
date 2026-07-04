using FlightBooking.MachineLearningModels;
using FlightBooking.MachineLearningRegressionModels;

namespace FlightBooking.Services.MachineLearningServices.FlightDataServices
{
    public interface IFlightDataService
    {
        Task<List<FlightRawData>> GetAllAsync();
        Task<List<FlightData>> ConvertToMlDataAsync();
        Task<List<FlightRegressionData>> ConvertToRegressionDataAsync();
    }
}
