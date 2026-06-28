using AutoMapper;
using FlightBooking.DTOs.BookingDTOs;
using FlightBooking.DTOs.FlightDTOs;
using FlightBooking.Entities;

namespace FlightBooking.Mapping
{
    public class GenerelMapping : Profile
    {
        public GenerelMapping()
        {
            CreateMap<Flight, CreateFlightDto>().ReverseMap();
            CreateMap<Flight, GetFlightByIdDto>().ReverseMap();
            CreateMap<Flight, UpdateFlightDto>().ReverseMap();
            CreateMap<Flight, ResultFlightDto>().ReverseMap();

            CreateMap<Passenger, GetFlightByIdDto>().ReverseMap();
        }
    }
}
