using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Write.Categorias;

public class CreateReceitaDTO
{

    public string Titulo { get; set; } = default!;

    public string? Descricao { get; set; }

    public decimal Valor { get; set; }
    
    public DateTime Data { get; set; }
}
