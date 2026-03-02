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

    // atributo que será atualizado sempre que fizer um lançamento para acompanhar histórico do user
    public int TotalLancamentos { get; set; } = 0;

    [Required]
    public int CarteiraId { get; set; }
   
    [ForeignKey(nameof(CarteiraId))]
    public Carteira Carteira { get; set; }


    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpire { get; set; }
}
