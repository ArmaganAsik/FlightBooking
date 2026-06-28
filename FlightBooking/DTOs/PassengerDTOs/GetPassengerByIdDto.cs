namespace FlightBooking.DTOs.PassengerDTOs
{
    public class GetPassengerByIdDto
    {
        public string? PassengerId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string PassengerType { get; set; } // Adult, Child, Infant
        public bool? IsCheckedIn { get; set; }
        public string? TicketStatus { get; set; }
        public string SeatNumber { get; set; }
    }
}
