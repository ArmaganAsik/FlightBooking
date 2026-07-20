using FlightBooking.AIAgentServices.CityDetectorServices;
using FlightBooking.AIAgentServices.IntentDetectorServices;
using FlightBooking.AIAgentServices.OpenAIServices;
using FlightBooking.AIAgentServices.PromptBuilderServices;
using FlightBooking.AIAgentTools.WeatherTool;
using FlightBooking.DTOs.AIAgentDTOs;

namespace FlightBooking.AIAgentServices.TravelAgentServices
{
    public class TravelAgentService : ITravelAgentService
    {
        private readonly IOpenAIService _openAIService;
        private readonly ITravelPromptBuilderService _travelPromptBuilderService;
        private readonly ITravelIntentDetectorService _travelIntentDetectorService;
        private readonly IWeatherTool _weatherTool;
        private readonly ICityExtractorService _cityExtractorService;

        public TravelAgentService(IOpenAIService openAIService, ITravelPromptBuilderService travelPromptBuilderService, ITravelIntentDetectorService travelIntentDetectorService, IWeatherTool weatherTool, ICityExtractorService cityExtractorService)
        {
            _openAIService = openAIService;
            _travelPromptBuilderService = travelPromptBuilderService;
            _travelIntentDetectorService = travelIntentDetectorService;
            _weatherTool = weatherTool;
            _cityExtractorService = cityExtractorService;
        }

        public async Task<AgentResponseDto> AskAgentAsync(string prompt)
        {
            TravelIntent intent = _travelIntentDetectorService.Detect(prompt);

            string intentInstruction;

            string city = await _cityExtractorService.ExtractCityAsync(prompt);

            switch (intent)
            {
                case TravelIntent.Weather:
                    WeatherResultDto weatherResult = await _weatherTool.GetWeatherAsync("Amsterdam");

                    intentInstruction =
                        $"Kullanıcı hava durumu bilgisi istiyor. " +
                        $"Gerçek hava durumu verisi: " +
                        $"Şehir: {weatherResult.City}, " +
                        $"Sıcaklık: {weatherResult.Temperature}°C, " +
                        $"Durum: {weatherResult.Condition}, " +
                        $"Nem: %{weatherResult.Humidity}, " +
                        $"Rüzgar: {weatherResult.WindSpeed} km/s. " +
                        $"Bu verilere göre kullanıcıya seyahat ve kıyafet önerisi ver.";
                    break;

                case TravelIntent.Restaurant:
                    intentInstruction =
                        "Kullanıcı restoran önerisi istiyor.";
                    break;

                case TravelIntent.Hotel:
                    intentInstruction =
                        "Kullanıcı otel önerisi istiyor.";
                    break;

                default:
                    intentInstruction =
                        "Kullanıcının seyahatle ilgili sorusuna yardımcı ol.";
                    break;
            }

            string finalPrompt = _travelPromptBuilderService.BuildPrompt(
                $"{intentInstruction}\n\nKullanıcının gerçek sorusu:\n{prompt}");

            AgentResponseDto result = await _openAIService.GetResponseAsync(finalPrompt);

            result.Intent = intent.ToString();

            return result;
        }
    }
}
