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

    [PasswordPropertyText]
    public string Password { get; set; } = default!;
}
