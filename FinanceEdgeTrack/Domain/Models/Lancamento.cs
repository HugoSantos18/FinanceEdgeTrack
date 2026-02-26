using Mapster;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceEdgeTrack.Domain.Validations;


namespace FinanceEdgeTrack.Domain.Models;

[Table("Lancamento")]
public class Lancamento
{
    public Guid LancamentoId { get; set; } = Guid.NewGuid();

    [Required]
    public DateTime DataLancamento { get; set; }

    [OptionalCategoryAtributte]
    public Guid? ReceitaId { get; set; }

    [ForeignKey(nameof(ReceitaId))]
    public Receita? Receita { get; set; }

    [OptionalCategoryAtributte]
    public Guid? DespesaId { get; set; }

    [ForeignKey(nameof(DespesaId))]
    public Despesa? Despesa { get; set; }


    [Required]
    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }  

}
