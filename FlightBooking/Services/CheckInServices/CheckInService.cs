using AutoMapper;
using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.CheckInDTOs;
using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.DTOs.PassengerDTOs;
using FlightBooking.Entities;
using FlightBooking.Settings;
using Humanizer;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FlightBooking.Services.CheckInServices
{
    public class CheckInService : ICheckInService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Booking> _bookingCollection;
        private readonly IMongoCollection<CheckIn> _checkInCollection;
        private readonly IMongoCollection<Flight> _flightCollection;

        public CheckInService(IDatabaseSettings settings, IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _bookingCollection = database.GetCollection<Booking>(settings.BookingCollectionName);
            _checkInCollection = database.GetCollection<CheckIn>(settings.CheckInCollectionName);
            _flightCollection = database.GetCollection<Flight>(settings.FlightCollectionName);
            _mapper = mapper;
        }

        public async Task CompleteCheckInAsync(CompleteCheckInDto completeCheckInDto)
        {
            // BoardingPassNumber burada generate edilir
            string boardingPassNumber = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            // 1. CheckIn kaydı oluştur
            CheckIn checkIn = new CheckIn
            {
                CheckInId = ObjectId.GenerateNewId().ToString(),
                PassengerId = completeCheckInDto.PassengerId,
                FlightId = completeCheckInDto.FlightId,
                PnrNumber = completeCheckInDto.PnrNumber,
                CheckInDate = DateTime.UtcNow,
                SeatNumber = completeCheckInDto.SeatNumber,
                CheckInExtras = completeCheckInDto.CheckInExtras ?? new List<CheckInExtra>(),
                ExtraTotalPrice = completeCheckInDto.ExtraTotalPrice,
                BoardingPassNumber = boardingPassNumber
            };

            await _checkInCollection.InsertOneAsync(checkIn);

            // 2. Booking'deki Passenger'ı güncelle
            var filter = Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(b => b.FlightId, completeCheckInDto.FlightId),
                Builders<Booking>.Filter.ElemMatch(b => b.Passengers, p => p.PassengerId == completeCheckInDto.PassengerId)
            );

            var update = Builders<Booking>.Update
                .Set("Passengers.$.IsCheckedIn", true)
                .Set("Passengers.$.SeatNumber", completeCheckInDto.SeatNumber);

            await _bookingCollection.UpdateOneAsync(filter, update);
        }

        public async Task<CheckInDataDto> GetCheckInDataAsync(string passengerId)
        {
            Booking booking = await _bookingCollection.Find(b => b.Passengers.Any(p => p.PassengerId == passengerId)).FirstOrDefaultAsync();

            Passenger passenger = booking.Passengers.FirstOrDefault(p => p.PassengerId == passengerId);

            Flight flight = await _flightCollection.Find(f => f.FlightId == booking.FlightId).FirstOrDefaultAsync();

            GetPassengerByIdDto passengerDto = new GetPassengerByIdDto
            {
                PassengerId = passenger.PassengerId,
                Name = passenger.Name,
                Surname = passenger.Surname,
                BirthDate = passenger.BirthDate,
                Gender = passenger.Gender,
                PassengerType = passenger.PassengerType,
                IsCheckedIn = passenger.IsCheckedIn,
                // Check-in için gereksiz olabilecekler:
                // TicketStatus = passenger.TicketStatus,
                // PaymentStatus = passenger.PaymentStatus
            };

            GetFlightByIdDto flightDto = _mapper.Map<GetFlightByIdDto>(flight);

            return new CheckInDataDto
            {
                PnrNumber = booking.PnrNumber,
                Passenger = passengerDto,
                Flight = flightDto
            };
        }
    }
}
