using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Models.Abstract;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;

public class LancamentoDTO
{
    public DateTime Data { get; set; }
    public int CategoriaId { get; set; } = default!;
}
