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

            string? city = null;
            WeatherResultDto? weatherResult = null;

            switch (intent)
            {
                case TravelIntent.Weather:
                    {
                        city = await _cityExtractorService.ExtractCityAsync(prompt);

                        if (string.IsNullOrWhiteSpace(city))
                        {
                            intentInstruction =
                                "Kullanıcı hava durumu bilgisi istiyor ancak şehir belirtmemiş. " +
                                "Kullanıcıdan hangi şehrin hava durumunu öğrenmek istediğini sor.";

                            break;
                        }

                        weatherResult =
                            await _weatherTool.GetWeatherAsync(city);

                        string forecastText = string.Join(
                            "\n",
                            weatherResult.Forecasts.Select(x =>
                                $"{x.Day}: En düşük {x.Low}°C, " +
                                $"en yüksek {x.High}°C, durum: {x.Condition}"));

                        intentInstruction =
                            $"Kullanıcı hava durumu bilgisi istiyor.\n\n" +
                            $"Weather Tool tarafından sağlanan gerçek hava durumu verileri:\n" +
                            $"Şehir: {weatherResult.City}\n" +
                            $"Ülke: {weatherResult.Country}\n" +
                            $"Saat Dilimi: {weatherResult.TimeZoneId}\n" +
                            $"Sıcaklık: {weatherResult.Temperature}°C\n" +
                            $"Durum: {weatherResult.Condition}\n" +
                            $"Nem: %{weatherResult.Humidity}\n" +
                            $"Rüzgar: {weatherResult.WindSpeed} km/sa, {weatherResult.WindDirection}\n" +
                            $"Görüş mesafesi: {weatherResult.Visibility} km\n" +
                            $"Basınç: {weatherResult.Pressure} hPa\n" +
                            $"Gün doğumu: {weatherResult.Sunrise}\n" +
                            $"Gün batımı: {weatherResult.Sunset}\n\n" +
                            $"Gelecek gün tahminleri:\n{forecastText}\n\n" +
                            $"Yalnızca Weather Tool tarafından sağlanan verileri kullan. " +
                            $"Hava durumu veya sıcaklık uydurma. " +
                            $"Kullanıcının sorusuna göre kıyafet, şemsiye ve seyahat önerisi ver.";

                        break;
                    }

                case TravelIntent.Restaurant:
                    intentInstruction =
                        "Kullanıcı restoran önerisi istiyor.";
                    break;

                case TravelIntent.Hotel:
                    intentInstruction =
                        "Kullanıcı otel önerisi istiyor.";
                    break;

                case TravelIntent.Transportation:
                    intentInstruction =
                        "Kullanıcı ulaşım seçenekleri hakkında bilgi istiyor.";
                    break;

                case TravelIntent.Currency:
                    intentInstruction =
                        "Kullanıcı döviz kuru bilgisi istiyor.";
                    break;

                case TravelIntent.Itinerary:
                    intentInstruction =
                        "Kullanıcı seyahat planı veya rota hazırlanmasını istiyor.";
                    break;

                case TravelIntent.Attraction:
                    intentInstruction =
                        "Kullanıcı gezilecek yer önerileri istiyor.";
                    break;

                default:
                    intentInstruction =
                        "Kullanıcının seyahatle ilgili sorusuna yardımcı ol.";
                    break;
            }

            string finalPrompt = _travelPromptBuilderService.BuildPrompt(
                $"{intentInstruction}\n\nKullanıcının gerçek sorusu:\n{prompt}");

            AgentResponseDto result =
                await _openAIService.GetResponseAsync(finalPrompt);

            result.Intent = intent.ToString();

            result.City = city;
            result.Weather = weatherResult;

            return result;
        }
    }
}
