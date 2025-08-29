using System.ComponentModel.DataAnnotations;

namespace GloboClima.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "Username é obrigatório")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password é obrigatório")]
    public string Password { get; set; } = string.Empty;
}