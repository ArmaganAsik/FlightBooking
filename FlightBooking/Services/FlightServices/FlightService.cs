using AutoMapper;
using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.DTOs.PassengerDTOs;
using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.FlightServices
{
    public class FlightService : IFlightService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Flight> _flightCollection;
        private readonly IMongoCollection<Booking> _bookingCollection;

        public FlightService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            MongoClient client = new MongoClient(databaseSettings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(databaseSettings.DatabaseName);
            _flightCollection = database.GetCollection<Flight>(databaseSettings.FlightCollectionName);
            _bookingCollection = database.GetCollection<Booking>(databaseSettings.BookingCollectionName);
            _mapper = mapper;
        }
        public async Task CreateFlightAsync(CreateFlightDto createFlightDto)
        {
            Flight value = _mapper.Map<Flight>(createFlightDto);
            await _flightCollection.InsertOneAsync(value);
        }

        public async Task DeleteFlightAsync(string id)
        {
            await _flightCollection.DeleteOneAsync(x => x.FlightId == id);
        }

        public async Task<List<ResultFlightDto>> GetAllFlightsAsync()
        {
            List<Flight> values = await _flightCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultFlightDto>>(values);
        }

        public async Task<GetFlightByIdDto> GetFlightByIdAsync(string id)
        {
            Flight value = await _flightCollection.Find(x => x.FlightId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetFlightByIdDto>(value);
        }

        public async Task UpdateFlightAsync(UpdateFlightDto updateFlightDto)
        {
            Flight value = _mapper.Map<Flight>(updateFlightDto);
            await _flightCollection.FindOneAndReplaceAsync(x => x.FlightId == updateFlightDto.FlightId, value);
        }

        public async Task<List<ResultBookingDto>> GetFlightBookingsAsync(string id)
        {
            List<Booking> bookings = await _bookingCollection.Find(x => x.FlightId == id).ToListAsync();
            return bookings.Select(b => new ResultBookingDto
            {
                BookingId = b.BookingId,
                FlightId = b.FlightId,
                PnrNumber = b.PnrNumber,
                ContactName = b.ContactName,
                ContactEmail = b.ContactEmail,
                ContactPhone = b.ContactPhone,
                TotalPrice = b.TotalPrice,
                BookingDate = b.BookingDate,
                Status = b.Status,
                PaymentStatus = b.PaymentStatus,
                Passengers = b.Passengers?.Select(p => new ResultPassengerDto
                {
                    PassengerId = p.PassengerId,
                    Name = p.Name,
                    Surname = p.Surname,
                    Gender = p.Gender,
                    PassengerType = p.PassengerType,
                    IsCheckedIn = p.IsCheckedIn,
                    TicketStatus = p.TicketStatus,
                    SeatNumber = p.SeatNumber,
                    //PnrNumber = b.PnrNumber,
                    //PaymentStatus = b.PaymentStatus
                }).ToList()
            }).ToList();
        }
    }
}
