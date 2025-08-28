using GloboClima.Models;

namespace GloboClima.Services;

public interface ICountryService
{
    /// <summary>
    /// Obtém um país pelo nome.
    /// </summary>
    /// <param name="countryName">Nome do país a ser obtido.</param>
    /// <returns>Detalhes do país.</returns>
    /// <exception cref="HttpRequestException">Lançado quando ocorre um erro na solicitação HTTP.</exception>
    Task<CountryResponse[]> GetCountryByNameAsync(string countryName);
    
    /// <summary>
    /// Obtém uma lista paginada de todos os países.
    /// </summary>
    /// <returns>Lista paginada de países.</returns>
    /// <exception cref="HttpRequestException">Lançado quando ocorre um erro na solicitação HTTP.</exception>
    Task<CountryResponse[]> GetAllCountriesAsync();
    
    /// <summary>
    /// Obtém um país pelo nome com informações de clima.
    /// </summary>
    /// <param name="countryName">Nome do país a ser obtido.</param>
    /// <returns>Detalhes do país com informações de clima.</returns>
    /// <exception cref="ArgumentException">Lançado quando o país não é encontrado.</exception>
    Task<CountryWithWeatherResponse> GetCountryWithWeatherAsync(string countryName);
}