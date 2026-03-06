using FinanceEdgeTrack.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Application.Dtos.Write.Carteira;

public class CreateCarteiraDTO
{
    [Required]
    public string? UserId { get; set; }

    public decimal Saldo { get; set; } = default!;
}
