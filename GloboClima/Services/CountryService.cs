using GloboClima.Models;
using System.Text.Json;

namespace GloboClima.Services;

public class CountryService : ICountryService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CountryService> _logger;
    private readonly string _openweathermapApi = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric&lang=pt_br";
    private readonly string _restcountriesApi = "https://restcountries.com/v3.1/{0}";

    public CountryService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<CountryService> logger
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = _configuration["APIKEY"] ?? string.Empty;
    }

    public async Task<CountryResponse[]> GetCountryByNameAsync(string countryName)
    {
        var url = string.Format(_restcountriesApi, $"name/{countryName}");

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        var options = jsonSerializerOptions;

        return JsonSerializer.Deserialize<CountryResponse[]>(json, options) ?? [];
    }

    public async Task<CountryResponse[]> GetAllCountriesAsync()
    {
        var url = string.Format(_restcountriesApi, "all?fields=name,capital,population,region,flags");

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<CountryResponse[]>(json, options) ?? [];
    }

    public async Task<CountryWithWeatherResponse> GetCountryWithWeatherAsync(string countryName)
    {
        var countries = await GetCountryByNameAsync(countryName);
        var country = countries.FirstOrDefault();

        if (country == null)
            throw new ArgumentException($"País '{countryName}' não encontrado");

        var capital = country.Capital?.FirstOrDefault();
        WeatherInfo? weatherInfo = null;

        if (!string.IsNullOrEmpty(capital))
        {
            try
            {
                var weather = await GetCurrentWeatherAsync(capital);
                weatherInfo = new WeatherInfo
                {
                    Temperature = Math.Round(weather.Main.Temp, 1),
                    FeelsLike = Math.Round(weather.Main.Feels_Like, 1),
                    MinTemp = Math.Round(weather.Main.Temp_Min, 1),
                    MaxTemp = Math.Round(weather.Main.Temp_Max, 1),
                    Humidity = weather.Main.Humidity,
                    Description = weather.Weather[0].Description,
                    Main = weather.Weather[0].Main
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Não foi possível obter dados climáticos para {Capital}", capital);
            }
        }

        return new CountryWithWeatherResponse
        {
            Name = country.Name.Common,
            OfficialName = country.Name.Official,
            Capital = capital ?? "N/A",
            Population = country.Population,
            Region = country.Region,
            Subregion = country.Subregion,
            Currencies = country.Currencies?.Values.Select(c => new CurrencyInfo
            {
                Name = c.Name,
                Symbol = c.Symbol
            }).ToList() ?? [],
            Flag = country.Flags?.Png ?? string.Empty,
            Timezones = country.Timezones?.ToList() ?? [],
            Weather = weatherInfo
        };
    }

    #region Métodos Privados

    private async Task<WeatherResponse> GetCurrentWeatherAsync(string city)
    {
        var url = string.Format(_openweathermapApi, city, _apiKey);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<WeatherResponse>(json, options) ?? new WeatherResponse();
    }

    #endregion
}