using FinanceEdgeTrack.Domain.Models.Abstract;
using FinanceEdgeTrack.Enum;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FinanceEdgeTrack.Domain.Models;

public class Meta : Categoria
{
    [Required(ErrorMessage = "É obrigatório informar o valor alvo da Meta para alcança-lá. :)")]
    [Range(1, double.MaxValue)]
    public decimal ValorAlvo { get; set; }

    [Range(1, double.MaxValue)]
    public decimal UltimoDepositoEmReais { get; set; } = default!; 
    
    public DateTime DataUltimoDeposito { get; set; }

    [Range(0.0, 0.1)]
    public float PorcentagemAtual { get; set; }

    [Range(0.0, 0.1)]
    public float PorcentagemRestante { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ValorRestante { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime DataAlvo { get; set; }

    public Status Status { get; set; } = default!;

    public bool Concluida { get; set; } = default!;

    public List<AporteMetas>? Aportes { get; set; } = new();

}
