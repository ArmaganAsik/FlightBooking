using FlightBooking.MachineLearningRegressionModels;

namespace FlightBooking.Services.MachineLearningServices.Prediction
{
    public interface IFlightRegressionService
    {
        void Train(List<FlightRegressionData> dataList);
        FlightRegressionPrediction Predict(FlightRegressionData input);
    }
}
