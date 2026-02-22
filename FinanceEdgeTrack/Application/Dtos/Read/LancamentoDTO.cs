using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Models.Abstract;

namespace FinanceEdgeTrack.Application.Dtos.Read;

public class LancamentoDTO
{
    public DateTime Data { get; set; }
    public Categoria Categoria { get; set; } = default!;
    public AplicationUser User { get; set; } = default!;
}
