namespace GloboClima.Models;

public class WeatherResponse
{
    public Main Main { get; set; } = new();
    public Weather[] Weather { get; set; } = [];
    public string Name { get; set; } = string.Empty;
}

public class Main
{
    public double Temp { get; set; }
    public double Feels_Like { get; set; }
    public double Temp_Min { get; set; }
    public double Temp_Max { get; set; }
    public int Humidity { get; set; }
}

public class Weather
{
    public string Main { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}