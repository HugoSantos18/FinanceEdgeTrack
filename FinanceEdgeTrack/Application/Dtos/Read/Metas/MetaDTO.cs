using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Metas;

public class MetaDTO 
{
    public Guid MetaId { get; set; } 

    public string Titulo { get; set; } = default!;
    
    public string? Descricao { get; set; }

    public decimal ValorAlvo { get; set; }

    public decimal ValorRestante { get; }

    [Range(0, 100)]
    public decimal PorcentagemAtual { get; set; }
    
    public DateTime DataInicio { get; set; }
    
    public DateTime DataAlvo { get; set; }
    
    public Status Status { get; set; }

    public List<AporteMetasDTO> Aportes { get;}
}
