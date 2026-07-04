using FlightBooking.DTOs.NoShowDTOs;

namespace FlightBooking.Services.MachineLearningServices.NoShowDataServices
{
    public interface IOverbookingRecommendationService
    {
        Task<OverbookingRecommendationResultDto> GenerateRecommendationAsync(string flightDate, string flightSlot, int passengerCount, int capacity);
        Task<List<OverbookingForecastResultDto>> PredictJanuary2027Async();
        float ConvertFlightSlotToNumber(string slot);
    }
}
