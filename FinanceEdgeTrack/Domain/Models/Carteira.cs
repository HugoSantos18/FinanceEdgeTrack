using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Models;


[Table("Carteira")]
public class Carteira
{
    [Key]
    [Required]
    public Guid CarteiraId { get; private set; }

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public decimal Saldo { get; set; }

    public ICollection<Meta> Metas { get; private set; } = [];
    public ICollection<Receita> Receitas { get; private set; } = [];
    public ICollection<Despesa> Despesas { get; private set; } = [];

    public static Carteira CriarCarteira(string userId)
    {
        return new Carteira
        {
            CarteiraId = Guid.NewGuid(),
            UserId = userId,
            Saldo = 0
        };
    }

    public decimal AdicionarSaldo(decimal valor) => Saldo += valor;

    public decimal DescontarSaldo(decimal valor)
    {
        if (Saldo < valor) throw new InvalidOperationException("Saldo insuficiente");
        
        return Saldo -= valor;
    }
}
