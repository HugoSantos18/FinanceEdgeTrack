using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Domain.Models;

public class AplicationUser : IdentityUser
{
    [Required]
    [Range(0, 999999999)]
    public decimal ValorTotal { get; set; } = default!;

    [Range(0, 999999999)]
    public decimal ValorTotalInvestido { get; set; } = default!;

    [Range(0, 999999999)]
    public decimal ValorTotalGasto { get; set; } = default!;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}
