using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    public class CheckIn
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CheckInId { get; set; }
        public string PassengerId { get; set; }
        public string FlightId { get; set; }
        public string PnrNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public string SeatNumber { get; set; }
        public decimal ExtraTotalPrice { get; set; }
        public List<CheckInExtra> CheckInExtras { get; set; }
        public string? BoardingPassNumber { get; set; }
    }
}