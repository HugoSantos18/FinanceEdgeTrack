using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Write.Auth;

public class RegisterModelUserDTO
{
    [Required]
    [MaxLength(20)]
    public string UserName { get; set; } = default!;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; } = default!;

    [Required] 
    [PasswordPropertyText]
    public string ConfirmPassword { get; set; } = default!; 

    [Phone]
    public string Telefone { get; set; } = default!;

    [Required]
    [MaxLength(14)]
    public string CPF { get; set; } = default!;

    public DateTime DataNascimento { get; set; }
}
