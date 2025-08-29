using Microsoft.AspNetCore.Mvc;
using GloboClima.Models;
using GloboClima.Services;

namespace GloboClima.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <returns>Token JWT se login for bem-sucedido</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var result = await _authService.LoginAsync(request);
            //if (result == null)
            //{
            //    return Unauthorized(new { message = "Credenciais inválidas" });
            //}

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer login");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    /// <param name="request">Dados de cadastro</param>
    /// <returns>Sucesso ou erro no cadastro</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var success = await _authService.RegisterAsync(request);
            //if (!success)
            //{
            //    return Conflict(new { message = "Usuário já existe" });
            //}

            return Created("", new { message = "Usuário criado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}