using FinanceEdgeTrack.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read;

public class AporteMetasDTO
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "É necessário informar um valor para o aporte.")]
    [Range(1, double.MaxValue)]
    public decimal Valor { get; set; }

    public Meta? Meta { get; set; } 
}
