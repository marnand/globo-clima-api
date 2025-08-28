using Microsoft.AspNetCore.Mvc;
using GloboClima.Models;
using GloboClima.Services;

namespace GloboClima.Controllers;

[ApiController]
[Route("[controller]")]
public class CountryController : ControllerBase
{
    private readonly ILogger<CountryController> _logger;
    private readonly ICountryService _countryService;

    /// <summary>
    /// Controller para operações relacionadas a países.
    /// </summary>
    /// <param name="logger">Logger para registrar eventos e erros.</param>
    /// <param name="countryService">Serviço para operações relacionadas a países.</param>
    public CountryController(ILogger<CountryController> logger, ICountryService countryService)
    {
        _logger = logger;
        _countryService = countryService;
    }

    /// <summary>
    /// Obtém um país pelo nome.
    /// </summary>
    /// <param name="countryName">Nome do país a ser obtido.</param>
    /// <returns>Detalhes do país.</returns>
    [HttpGet("{countryName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CountryResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountry(string countryName)
    {
        try
        {
            var countries = await _countryService.GetCountryByNameAsync(countryName);
            
            if (!countries.Any())
                return NotFound($"País '{countryName}' não encontrado");

            var country = countries.First();
            
            var result = new
            {
                Name = country.Name.Common,
                OfficialName = country.Name.Official,
                Capital = country.Capital?.FirstOrDefault() ?? "N/A",
                country.Population,
                country.Region,
                country.Subregion,
                Currencies = country.Currencies?.Values.Select(c => new { c.Name, c.Symbol }).ToList() ?? [],
                Flag = country.Flags?.Png,
                Timezones = country.Timezones?.ToList() ?? []
            };
            
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao buscar dados do país: {Country}", countryName);
            return BadRequest($"Erro ao buscar dados do país: {countryName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno do servidor");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
    
    /// <summary>
    /// Obtém um país pelo nome com informações de clima.
    /// </summary>
    /// <param name="countryName">Nome do país a ser obtido.</param>
    /// <returns>Detalhes do país com informações de clima.</returns>
    [HttpGet("{countryName}/weather")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CountryWithWeatherResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountryWithWeather(string countryName)
    {
        try
        {
            var result = await _countryService.GetCountryWithWeatherAsync(countryName);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao buscar dados do país: {Country}", countryName);
            return BadRequest($"Erro ao buscar dados do país: {countryName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno do servidor");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém uma lista paginada de todos os países.
    /// </summary>
    /// <returns>Lista paginada de países.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CountryWithWeatherResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var countries = await _countryService.GetAllCountriesAsync();
            
            var pagedCountries = countries
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(country => new
                {
                    Name = country.Name.Common,
                    Capital = country.Capital?.FirstOrDefault() ?? "N/A",
                    country.Population,
                    country.Region,
                    Flag = country.Flags?.Png
                })
                .ToList();
            
            var result = new
            {
                TotalCount = countries.Length,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)countries.Length / pageSize),
                Countries = pagedCountries
            };
            
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao buscar lista de países");
            return BadRequest("Erro ao buscar lista de países");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno do servidor");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}