using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Models;


[Table("Carteira")]
public class Carteira
{
    [Key]
    public int CarteiraId { get; set; }

    [Required]
    public string? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    [Required]
    public decimal Saldo { get; set; } = default!;


    public decimal AdicionarSaldo(decimal valor)
    {
        return Saldo += valor;
    }
}
