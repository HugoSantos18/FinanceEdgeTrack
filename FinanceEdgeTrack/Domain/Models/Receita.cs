using Mapster;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Domain.Models;

public class Receita
{
    public Guid ReceitaId { get; private set; } = Guid.NewGuid();

    [Required(ErrorMessage = "É necessário um título para a categoria")]
    public string Titulo { get; set; } = default!;

    public string? Descricao { get; set; }

    [Required(ErrorMessage = "É obrigatório informar o valor da receita.")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal Valor { get; set; }

    public DateTime Data { get; set; }
}
