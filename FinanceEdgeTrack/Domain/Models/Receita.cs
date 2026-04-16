using Mapster;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinanceEdgeTrack.Domain.Models;

public class Receita
{
    public Guid ReceitaId { get; private set; } = Guid.NewGuid();
    public Guid CarteiraId { get; set; }

    [Required(ErrorMessage = "É necessário um título para a categoria")]
    public string Titulo { get; set; } = default!;

    public string? Descricao { get; set; }

    [JsonIgnore]
    public Carteira? Carteira { get; private set; }

    [Required(ErrorMessage = "É obrigatório informar o valor da receita.")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal Valor { get; set; }

    public DateTime Data { get; set; }
    
    public Receita(decimal valor)
    {
        Valor = valor;
    }
}
