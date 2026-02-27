using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Categorias;

public class DespesaDTO 
{
    public Guid DespesaId { get; set; } 

    public string Titulo { get; set; } = default!;

    public string? Descricao { get; set; }

    public decimal Valor { get; set; }

    public bool Fixa { get; set; }

    public DateTime Data { get; set; }
}
