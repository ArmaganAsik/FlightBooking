using FlightBooking.DTOs.CheckInDTOs;

namespace FlightBooking.Services.CheckInServices
{
    public interface ICheckInService
    {
        Task CompleteCheckInAsync(CompleteCheckInDto completeCheckInDto);
        Task<CheckInDataDto> GetCheckInDataAsync(string passengerId);
    }
}
