using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Models;


[Table("Carteira")]
public class Carteira
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public int CarteiraId { get; private set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; private set; }

    [Required]
    public decimal Saldo { get; set; } = default!;


    public decimal AdicionarSaldo(decimal valor)
    {
        return Saldo += valor;
    }

    public decimal DescontarSaldo(decimal valor)
    {
        return Saldo -= valor;
    }
}
