using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Metas;

public class MetaDTO : CategoriaBaseDTO
{
    public decimal ValorAlvo { get; set; }

    public decimal ValorRestante { get; set; }

    [Range(0, 100)]
    public decimal PorcentagemAtual { get; set; }
    
    public DateTime DataInicio { get; set; }
    
    public DateTime DataAlvo { get; set; }
    
    public Status Status { get; set; }

    public List<AporteMetasDTO> Aportes { get; set; } = new();

}
