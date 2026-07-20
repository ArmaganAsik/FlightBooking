namespace FlightBooking.AIAgentServices.IntentDetectorServices
{
    public interface ITravelIntentDetectorService
    {
        TravelIntent Detect(string prompt);
    }

    public enum TravelIntent
    {
        Unknown,
        Restaurant,
        Weather,
        Hotel,
        Transportation,
        Currency,
        Itinerary,
        Attraction
    }
}
