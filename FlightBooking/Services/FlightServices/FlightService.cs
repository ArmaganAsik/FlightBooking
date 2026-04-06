using AutoMapper;
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

        public async Task<List<PassengerListItemDto>> GetFlightPassengersAsync(string id)
        {
            List<Booking> bookings = await _bookingCollection.Find(x => x.FlightId == id).ToListAsync();

            List<PassengerListItemDto> passengers = bookings.SelectMany(y => y.Passengers.Select(p => new PassengerListItemDto
            {
                Name = p.Name,
                Surname = p.Surname,
                Email = y.ContactEmail,
                Gender = p.Gender,
                PassengerType = p.PassengerType,
                Pnr = y.BookingId,
                Phone = y.ContactPhone,
                SeatNumber = p.SeatNumber,
                CheckInStatus = p.CheckInStatus,
                PaymentStatus = y.PaymentStatus,
                TicketStatus = p.TicketStatus
            })).ToList();

            return passengers;
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
    }
}
