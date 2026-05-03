using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.PassengerDTOs;
using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace FlightBooking.Services.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly IMongoCollection<Booking> _bookingCollection;
        private readonly IMongoCollection<Flight> _flightCollection;

        public BookingService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _bookingCollection = database.GetCollection<Booking>(settings.BookingCollectionName);
            _flightCollection = database.GetCollection<Flight>(settings.FlightCollectionName);
        }

        public async Task CreateBookingAsync(CreateBookingDto dto)
        {
            // 🔥 1. Flight çek
            var flight = await _flightCollection
                .Find(x => x.FlightId == dto.FlightId)
                .FirstOrDefaultAsync();

            //if (flight == null)
            //    throw new Exception("Uçuş bulunamadı");

            // 🔥 2. Yolcu sayısı
            var passengerCount = dto.Passengers.Count;

            //// 🔥 3. Koltuk kontrol
            //if (flight.AvailableSeats < passengerCount)
            //    throw new Exception("Yeterli koltuk yok");

            // 🔥 4. Passenger mapping
            var passengers = dto.Passengers.Select(x => new Passenger
            {
                PassengerId = ObjectId.GenerateNewId().ToString(), //Army 04.05.2026
                Name = x.Name,
                Surname = x.Surname,
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                PassengerType = x.PassengerType
            }).ToList();

            // 🔥 5. Fiyat hesaplama
            var totalPrice = passengerCount * flight.BasePrice;

            var pnr = await GenerateUniquePnrAsync();

            // 🔥 6. Booking oluştur
            var booking = new Booking
            {
                FlightId = dto.FlightId,
                Passengers = passengers,

                ContactName = dto.ContactName,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,

                TotalPrice = totalPrice,
                BookingDate = DateTime.Now,
                Status = "Confirmed",
                PnrNumber = pnr
            };

            await _bookingCollection.InsertOneAsync(booking);

            //// 🔥 7. Koltuk düş
            //var update = Builders<Flight>.Update
            //    .Inc(x => x.AvailableSeats, -passengerCount);

            //await _flightCollection.UpdateOneAsync(
            //    x => x.FlightId == dto.FlightId,
            //    update
            //);
        }

        private async Task<string> GenerateUniquePnrAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rnd = new Random();

            string pnr;
            bool exists;

            do
            {
                pnr = new string(Enumerable.Repeat(chars, 6).Select(s => s[rnd.Next(s.Length)]).ToArray());
                exists = await _bookingCollection.Find(x => x.PnrNumber == pnr).AnyAsync();
            } while (exists);

            return pnr;
        }

        public async Task<GetBookingByIdDto> GetBookingByPassengerIdAsync(string passengerId)
        {
            Booking booking = await _bookingCollection.Find(b => b.Passengers.Any(p => p.PassengerId == passengerId)).FirstOrDefaultAsync();

            GetBookingByIdDto bookingDto = new GetBookingByIdDto
            {
                BookingId = booking.BookingId.ToString(),
                FlightId = booking.FlightId,
                PnrNumber = booking.PnrNumber,
                Passengers = booking.Passengers,
                ContactName = booking.ContactName,
                ContactEmail = booking.ContactEmail,
                ContactPhone = booking.ContactPhone,
                TotalPrice = booking.TotalPrice,
                BookingDate = booking.BookingDate,
                Status = booking.Status,
                PaymentStatus = booking.PaymentStatus
            };

            return bookingDto;
        }
    }
}
