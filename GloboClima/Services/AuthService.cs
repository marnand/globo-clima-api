using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using GloboClima.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GloboClima.Services;

public class AuthService : IAuthService
{
    private readonly IDynamoDBContext _dynamoDbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IDynamoDBContext dynamoDbContext, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _dynamoDbContext = dynamoDbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await GetUserByUsernameAsync(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var token = GenerateJwtToken(user);
            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer login para o usuário {Username}", request.Username);
            throw;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        try
        {
            if (await UserExistsAsync(request.Username))
            {
                return false;
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _dynamoDbContext.SaveAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário {Username}", request.Username);
            throw;
        }
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        try
        {
            var user = await GetUserByUsernameAsync(username);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se usuário {Username} existe", username);
            throw;
        }
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSecret = _configuration["JWT_SECRET"] ?? "your-super-secret-jwt-key-here-make-it-long-and-secure";
        var key = Encoding.ASCII.GetBytes(jwtSecret);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task<User?> GetUserByUsernameAsync(string username)
    {
        var scanConditions = new List<ScanCondition>
        {
            new ScanCondition("Username", ScanOperator.Equal, username)
        };
        
        var search = _dynamoDbContext.ScanAsync<User>(scanConditions);
        var users = await search.GetNextSetAsync();
        return users.FirstOrDefault();
    }
}