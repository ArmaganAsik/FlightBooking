using FlightBooking.AIAgentServices.CityDetectorServices;
using FlightBooking.AIAgentServices.GooglePlacesServices;
using FlightBooking.AIAgentServices.IntentDetectorServices;
using FlightBooking.AIAgentServices.OpenAIServices;
using FlightBooking.AIAgentServices.PromptBuilderServices;
using FlightBooking.AIAgentServices.TravelAgentServices;
using FlightBooking.AIAgentSettings;
using FlightBooking.AIAgentTools.WeatherTool;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.CheckInServices;
using FlightBooking.Services.FlightServices;
using FlightBooking.Services.MachineLearningServices.FlightDataServices;
using FlightBooking.Services.MachineLearningServices.NoShowDataServices;
using FlightBooking.Services.MachineLearningServices.Prediction;
using FlightBooking.Settings;
using Microsoft.Extensions.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ICheckInService, CheckInService>();
builder.Services.AddSingleton<IFlightMLService, FlightMLService>();
builder.Services.AddScoped<IFlightDataService, FlightDataService>();
builder.Services.AddSingleton<IFlightRegressionService, FlightRegressionService>();
builder.Services.AddScoped<INoShowService, NoShowService>();
builder.Services.AddScoped<IOverbookingRecommendationService, OverbookingRecommendationService>();
builder.Services.AddScoped<ITravelAgentService, TravelAgentService>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<ITravelPromptBuilderService, TravelPromptBuilderService>();
builder.Services.AddScoped<ITravelIntentDetectorService, TravelIntentDetectorService>();
builder.Services.AddScoped<IWeatherTool, WeatherTool>();
builder.Services.AddHttpClient<ICityExtractorService, CityExtractorService>();
builder.Services.AddHttpClient<IGooglePlacesService, GooglePlacesService>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettingsKey"));
builder.Services.AddScoped<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});

builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();