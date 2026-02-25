using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Models.Abstract;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;

public class LancamentoDTO
{
    [Required]
    public Guid Id { get; set; }
    
    public DateTime DataLancamento { get; set; }

    [Required]
    public int CategoriaId { get; set; }
    
    [Required]
    public string? UserId { get; set; }
}
