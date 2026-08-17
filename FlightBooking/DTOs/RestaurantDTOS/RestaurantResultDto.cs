namespace FlightBooking.DTOs.RestaurantDTOS
{
    public class RestaurantResultDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double? Rating { get; set; }
        public int? UserRatingCount { get; set; }
        public string PriceLevel { get; set; }
        public string GoogleMapsUrl { get; set; }
    }
}
