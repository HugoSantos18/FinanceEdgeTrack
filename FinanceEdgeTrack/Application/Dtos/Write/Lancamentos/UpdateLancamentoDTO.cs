using FinanceEdgeTrack.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Write.Lancamentos;

public class UpdateLancamentoDTO
{
    [Required]
    public Guid LancamentoId { get; set; }
    
    [Required]
    public DateTime DataLancamento { get; set; }

    [OptionalCategoryAtributte]
    public Guid? ReceitaId { get; set; }

    [OptionalCategoryAtributte]
    public Guid? DespesaId { get; set; }

    public string? UpdatedAt { get; set; }
}
