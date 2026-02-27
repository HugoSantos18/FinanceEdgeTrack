using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;

public class LancamentoDTO
{
    [Required]
    public Guid LancamentoId { get; set; }

    [Required]
    public DateTime DataLancamento { get; set; }

    [OptionalCategoryAtributte]
    public Guid? ReceitaId { get; set; }

    [OptionalCategoryAtributte]
    public Guid? DespesaId { get; set; }

    [Required]
    public string UserId { get; set; }
}
