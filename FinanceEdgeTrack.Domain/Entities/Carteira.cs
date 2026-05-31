using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Entities;


[Table("Carteira")]
public class Carteira
{
    [Key]
    [Required]
    public Guid CarteiraId { get; private set; }

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public decimal Saldo { get; private set; } 

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
}
