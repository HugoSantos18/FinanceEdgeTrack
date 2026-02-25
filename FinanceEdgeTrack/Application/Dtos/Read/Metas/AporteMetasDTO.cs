using FinanceEdgeTrack.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Metas;

public class AporteMetasDTO
{
    [Required(ErrorMessage = "É necessário informar um valor para o aporte.")]
    [Range(1, 99999999)]
    public decimal Valor { get; set; }

    [Required]
    public Guid MetaId { get; set; }
}
