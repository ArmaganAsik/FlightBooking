using FlightBooking.Entities;

namespace FlightBooking.DTOs.CheckInDTOs
{
    public class CompleteCheckInDto
    {
        public string PassengerId { get; set; }
        public string FlightId { get; set; }
        public string PnrNumber { get; set; }
        public string SeatNumber { get; set; }
        public List<CheckInExtra>? CheckInExtras { get; set; }
        public decimal ExtraTotalPrice { get; set; }
    }
}
