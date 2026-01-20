using FinanceEdgeTrack.Domain.Models.Abstract;
using FinanceEdgeTrack.Enum;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FinanceEdgeTrack.Domain.Models;

public class Meta : Categoria
{
    public decimal ValorAssociado { get; set; }

    public decimal UltimoDepositoEmReais { get; set; }  
    
    public DateTime DataUltimoDeposito { get; set; }
    
    public float PorcentagemAtual { get; set; }

    public float PorcentagemRestante { get; set; }

    public decimal ValorRestante { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime DataAlvo { get; set; }
    
    public Status Status { get; set; } = default!;

    public List<AporteMetas>? Aportes { get; set; } = new();

}
