using FlightBooking.MachineLearningModels;

namespace FlightBooking.Services.MachineLearningServices.Prediction
{
    public interface IFlightMLService
    {
        void Train(List<FlightData> dataList);
        FlightPrediction Predict(FlightData input);
    }
}
