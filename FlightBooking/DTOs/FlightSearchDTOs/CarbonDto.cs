namespace FlightBooking.DTOs.FlightSearchDTOs
{
    // Karbon emisyonu
    public class CarbonDto
    {
        public int Co2eKg { get; set; }             // kg cinsinden (170000 -> 170)
        public int DifferencePercent { get; set; }  // tipik rotaya göre % fark
    }
}