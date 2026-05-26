using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.DTOs.Write.Auth;

public class MakeAdminDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
