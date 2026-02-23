using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Metas;

public class MetaDTO : CategoriaBaseDTO
{
    [Required(ErrorMessage = "É obrigatório informar o valor da Meta para alcança-lá.")]
    [Range(1, double.MaxValue)]
    public decimal ValorAlvo { get; set; }

    public decimal ValorRestante { get; set; }

    [Range(0.0,0.1)]
    public float PorcentagemAtual { get; set; }
    
    public DateTime DataInicio { get; set; }
    
    public DateTime DataAlvo { get; set; }
    
    public Status Status { get; set; }

}
