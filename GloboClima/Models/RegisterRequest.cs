using System.ComponentModel.DataAnnotations;

namespace GloboClima.Models;

public class RegisterRequest
{
    [Required(ErrorMessage = "Username é obrigatório")]
    [MinLength(3, ErrorMessage = "Username deve ter pelo menos 3 caracteres")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password é obrigatório")]
    [MinLength(6, ErrorMessage = "Password deve ter pelo menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;
}