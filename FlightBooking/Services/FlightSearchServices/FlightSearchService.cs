using FlightBooking.DTOs.FlightSearchDTOs;
using FlightBooking.Models.FlightSearchModels;
using System.Text.Json;

namespace FlightBooking.Services.FlightSearchServices
{
    public class FlightSearchService : IFlightSearchService
    {
        private readonly HttpClient _client;

        public FlightSearchService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<FlightCardDto>> SearchAsync(
            string fromIata,
            string toIata,
            string outboundDate,
            int adults,
            string cabin,
            string currency)
        {
            List<FlightCardDto> cards = new List<FlightCardDto>();

            if (string.IsNullOrWhiteSpace(fromIata) || string.IsNullOrWhiteSpace(toIata))
                return cards;

            string travelClass = MapCabin(cabin);

            string url =
                $"https://google-flights2.p.rapidapi.com/api/v1/searchFlights" +
                $"?departure_id={Uri.EscapeDataString(fromIata)}" +
                $"&arrival_id={Uri.EscapeDataString(toIata)}" +
                $"&outbound_date={Uri.EscapeDataString(outboundDate)}" +
                $"&travel_class={travelClass}" +
                $"&adults={adults}" +
                $"&show_hidden=1" +
                $"&currency={Uri.EscapeDataString(currency)}" +
                $"&language_code=en-US&country_code=US&search_type=best";

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "x-rapidapi-key", "API_KEY_HERE" },
                    { "x-rapidapi-host", "google-flights2.p.rapidapi.com" },
                },
            };

            using HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string body = await response.Content.ReadAsStringAsync();

            FlightApiResponse parsed = JsonSerializer.Deserialize<FlightApiResponse>(body);
            FlightApiItineraries itin = parsed?.Data?.Itineraries;
            if (itin == null) return cards;

            List<FlightApiItinerary> all = new List<FlightApiItinerary>();
            if (itin.TopFlights != null) all.AddRange(itin.TopFlights);
            if (itin.OtherFlights != null) all.AddRange(itin.OtherFlights);

            foreach (FlightApiItinerary it in all)
            {
                FlightApiLeg firstLeg = it.Flights?.FirstOrDefault();
                FlightApiLeg lastLeg = it.Flights?.LastOrDefault();

                FlightCardDto card = new FlightCardDto
                {
                    // ---- özet ----
                    Airline = firstLeg?.Airline ?? "",
                    AirlineLogo = it.AirlineLogo ?? firstLeg?.AirlineLogo ?? "",
                    DepartureTime = ExtractClock(it.DepartureTime),
                    ArrivalTime = ExtractClock(it.ArrivalTime),
                    DepartureAirport = firstLeg?.DepartureAirport?.AirportCode ?? fromIata,
                    ArrivalAirport = lastLeg?.ArrivalAirport?.AirportCode ?? toIata,
                    DurationText = it.Duration?.Text ?? "",
                    Stops = it.Stops,
                    LayoverCities = it.Layovers?
                        .Where(l => !string.IsNullOrWhiteSpace(l.City))
                        .Select(l => l.City!)
                        .ToList() ?? new List<string>(),
                    Price = NormalizePrice(it.Price),

                    // ---- detay: segmentler ----
                    Segments = it.Flights?.Select(leg => new FlightSegmentDto
                    {
                        Airline = leg.Airline ?? "",
                        AirlineLogo = leg.AirlineLogo ?? "",
                        FlightNumber = leg.FlightNumber ?? "",
                        Aircraft = leg.Aircraft ?? "",
                        Legroom = leg.Legroom ?? "",
                        DurationText = leg.Duration?.Text ?? "",
                        DepartureTime = leg.DepartureAirport?.Time ?? "",
                        DepartureCode = leg.DepartureAirport?.AirportCode ?? "",
                        DepartureName = leg.DepartureAirport?.AirportName ?? "",
                        ArrivalTime = leg.ArrivalAirport?.Time ?? "",
                        ArrivalCode = leg.ArrivalAirport?.AirportCode ?? "",
                        ArrivalName = leg.ArrivalAirport?.AirportName ?? ""
                    }).ToList() ?? new List<FlightSegmentDto>(),

                    // ---- detay: aktarmalar ----
                    Layovers = it.Layovers?.Select(l => new LayoverDto
                    {
                        AirportCode = l.AirportCode ?? "",
                        AirportName = l.AirportName ?? "",
                        City = l.City ?? "",
                        DurationLabel = l.DurationLabel ?? ""
                    }).ToList() ?? new List<LayoverDto>(),

                    // ---- detay: bagaj ----
                    Bags = it.Bags == null ? null : new BagsDto
                    {
                        CarryOn = it.Bags.CarryOn,
                        Checked = it.Bags.Checked
                    },

                    // ---- detay: karbon (gram -> kg) ----
                    Carbon = it.CarbonEmissions == null ? null : new CarbonDto
                    {
                        Co2eKg = it.CarbonEmissions.Co2e / 1000,
                        DifferencePercent = it.CarbonEmissions.DifferencePercent
                    }
                };

                cards.Add(card);
            }

            return cards;
        }

        // "25-08-2026 07:30 AM" -> "07:30 AM"
        private static string ExtractClock(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            string[] parts = raw.Split(' ', 2);
            return parts.Length == 2 ? parts[1] : raw;
        }

        private static string NormalizePrice(JsonElement price)
        {
            return price.ValueKind switch
            {
                JsonValueKind.Number => price.GetRawText(),
                JsonValueKind.String => price.GetString() ?? "unavailable",
                _ => "unavailable"
            };
        }

        private static string MapCabin(string cabin) => cabin?.ToLower() switch
        {
            "business" => "BUSINESS",
            "first" => "FIRST",
            "premium" => "PREMIUM_ECONOMY",
            _ => "ECONOMY"
        };
    }
}
