using FinanceEdgeTrack.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Infrastructure.Identity;

/// <summary>
/// Atributos extras para serem adicionados ao IdentityUser, como o valor total, valor total investido, valor total gasto, carteira, CPF e data de nascimento.   
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required]
    [Range(0, 999999999)]
    public decimal ValorTotal { get; set; } = default!;

    [Range(0, 999999999)]
    public decimal ValorTotalInvestido { get; set; } = default!;

    [Range(0, 999999999)]
    public decimal ValorTotalGasto { get; set; } = default!;

    public Carteira Carteira { get; private set; } = null!;

    [Required]
    [MaxLength(14)]
    public string CPF { get; set; } = default!;

    public DateTime DataNascimento { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpire { get; set; }
}
