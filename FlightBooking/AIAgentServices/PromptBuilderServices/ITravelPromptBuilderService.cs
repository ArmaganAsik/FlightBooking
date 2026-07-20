namespace FlightBooking.AIAgentServices.PromptBuilderServices
{
    public interface ITravelPromptBuilderService
    {
        string BuildPrompt(string userPrompt);
    }
}
