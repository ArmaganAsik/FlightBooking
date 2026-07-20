namespace FlightBooking.AIAgentServices.CityDetectorServices
{
    public interface ICityExtractorService
    {
        Task<string> ExtractCityAsync(string city);
    }
}
