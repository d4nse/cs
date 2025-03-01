using System.Text.Json;
using static System.Console;

public class Program {
    private const string ApiKey = "be11722dd13045d6992215950250103"; // i dont care, throwaway api
    private static readonly HttpClient client = new HttpClient();

    public static async Task Main(string[] args) {
        if (args.Length == 0) {
            WriteLine("Usage: program <location>");
            return;
        }
        string loc = string.Concat(args[0][0].ToString().ToUpper(), args[0].AsSpan(1));
        WriteLine($"Searching weather for '{loc}'...");
        try {
            var forecast = await GetWeatherForecast(loc);
            WriteLine($"7-Day Weather Forecast for {forecast.location.name}:");
            foreach (var day in forecast.forecast.forecastday) {
                WriteLine($"{day.date} - " + $"Max: {day.day.maxtemp_c}°C / {day.day.maxtemp_f}°F, " +
                          $"Min: {day.day.mintemp_c}°C / {day.day.mintemp_f}°F");
            }
        } catch (Exception ex) { WriteLine($"Error: {ex.Message}"); }
    }

    private static async Task<ForecastResponse> GetWeatherForecast(string location) {
        string url = $"http://api.weatherapi.com/v1/forecast.json?key={ApiKey}&q={location}&days=7&aqi=no&alerts=no";
        var response = await client.GetStringAsync(url);
        return JsonSerializer.Deserialize<ForecastResponse>(response);
    }
}

public struct ForecastResponse {
    public Location location { get; set; }
    public Forecast forecast { get; set; }
}

public struct Location {
    public string name { get; set; }
}

public struct Forecast {
    public ForecastDay[] forecastday { get; set; }
}

public struct ForecastDay {
    public string date { get; set; }
    public Day day { get; set; }
}

public struct Day {
    public decimal maxtemp_c { get; set; }
    public decimal maxtemp_f { get; set; }
    public decimal mintemp_c { get; set; }
    public decimal mintemp_f { get; set; }
}
