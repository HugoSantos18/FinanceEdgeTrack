using Mapster;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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

    // atributo que será atualizado sempre que fizer um lançamento para acompanhar histórico do user
    public int TotalLancamentos { get; set; } = 0; 

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}
