using Mapster;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [Range(0, 999999999)]
    public decimal ValorTotal { get; set; } = default!;

    [Range(0, 999999999)]
    public decimal ValorTotalInvestido { get; set; } = default!;

    [Range(0, 999999999)]
    public decimal ValorTotalGasto { get; set; } = default!;

    public int TotalLancamentos { get; set; } = 0;

    public Carteira? Carteira { get; set; }

    [Required]
    [MaxLength(14)]
    public string CPF { get; set; } = default!;

    public DateTime DataNascimento { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpire { get; set; }
}
