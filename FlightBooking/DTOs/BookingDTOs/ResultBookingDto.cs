using FlightBooking.DTOs.PassengerDTOs;

namespace FlightBooking.DTOs.BookingDTOs
{
    public class ResultBookingDto
    {
        public string BookingId { get; set; }
        public string FlightId { get; set; }
        public string PnrNumber { get; set; }
        public List<ResultPassengerDto> Passengers { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
