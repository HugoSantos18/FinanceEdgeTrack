using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Auth;

public class ApplicationUserDTO
{
    [Required]
    [MaxLength(20)]
    public string UserName { get; set; } = default!;

    [EmailAddress]
    public string Email { get; set; } = default!;

    public string Telefone { get; set; } = default!;

    [Required]
    [MaxLength(14)]
    public string CPF { get; set; } = default!;

    public DateTime DataNascimento { get; set; }
}
