namespace GloboClima.Models;

public class CountryResponse
{
    public Name Name { get; set; } = new();
    public string[] Capital { get; set; } = [];
    public long Population { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Subregion { get; set; } = string.Empty;
    public Dictionary<string, Currency> Currencies { get; set; } = [];
    public Flags Flags { get; set; } = new();
    public string[] Timezones { get; set; } = [];
}

public class Name
{
    public string Common { get; set; } = string.Empty;
    public string Official { get; set; } = string.Empty;
}

public class Currency
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
}

public class Flags
{
    public string Png { get; set; } = string.Empty;
    public string Svg { get; set; } = string.Empty;
}

public class CountryWithWeatherResponse
{
    public string Name { get; set; } = string.Empty;
    public string OfficialName { get; set; } = string.Empty;
    public string Capital { get; set; } = string.Empty;
    public long Population { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Subregion { get; set; } = string.Empty;
    public List<string> Languages { get; set; } = [];
    public List<CurrencyInfo> Currencies { get; set; } = [];
    public string Flag { get; set; } = string.Empty;
    public List<string> Timezones { get; set; } = [];
    public WeatherInfo? Weather { get; set; } = null;
}

public class CurrencyInfo
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
}

public class WeatherInfo
{
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public double MinTemp { get; set; }
    public double MaxTemp { get; set; }
    public int Humidity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Main { get; set; } = string.Empty;
}