using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mapster;

namespace FinanceEdgeTrack.Domain.Models;

public class Despesa
{
    public Guid DespesaId { get; private set; } = Guid.NewGuid();
    public Guid CarteiraId { get; set; }

    [Required(ErrorMessage = "É necessário um título para a categoria")]
    public string Titulo { get; set; } = default!;
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "É obrigatório informar o valor da despesa.")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal Valor { get; set; }

    public bool Fixa { get; private set; }
    
    public DateTime Data { get; set; }


    public Despesa(decimal valor, bool fixa)
    {
        Valor = valor;
        Fixa = fixa;    
    }
}
