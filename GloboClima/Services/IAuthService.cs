using GloboClima.Models;

namespace GloboClima.Services;

public interface IAuthService
{
    /// <summary>
    /// Realiza o login do usuário.
    /// </summary>
    /// <param name="request">Dados de login do usuário.</param>
    /// <returns>Resposta de login contendo o token JWT e informações do usuário.</returns>
    /// <exception cref="Exception">Lançada quando ocorre um erro inesperado.</exception>
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    
    /// <summary>
    /// Registra um novo usuário.
    /// </summary>
    /// <param name="request">Dados de registro do usuário.</param>
    /// <returns>True se o registro for bem-sucedido, false caso contrário.</returns>
    /// <exception cref="Exception">Lançada quando ocorre um erro inesperado.</exception>
    Task<bool> RegisterAsync(RegisterRequest request);
    
    /// <summary>
    /// Verifica se um usuário existe.
    /// </summary>
    /// <param name="username">Nome de usuário a ser verificado.</param>
    /// <returns>True se o usuário existir, false caso contrário.</returns>
    /// <exception cref="Exception">Lançada quando ocorre um erro inesperado.</exception>
    Task<bool> UserExistsAsync(string username);
}