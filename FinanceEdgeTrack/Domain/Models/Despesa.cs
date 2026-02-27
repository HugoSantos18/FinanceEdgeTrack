using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mapster;

namespace FinanceEdgeTrack.Domain.Models;

public class Despesa
{
    public Guid DespesaId { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "É necessário um título para a categoria")]
    public string Titulo { get; set; } = default!;
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "É obrigatório informar o valor da despesa.")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal Valor { get; set; }

    public bool Fixa { get; set; }
    
    public DateTime Data { get; set; }

}
