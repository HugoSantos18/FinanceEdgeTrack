using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Categorias;

public class CategoriaBaseDTO
{
    public Guid CategoriaId { get; set; }
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public string TipoCategoria { get; set; } = default!;

}
