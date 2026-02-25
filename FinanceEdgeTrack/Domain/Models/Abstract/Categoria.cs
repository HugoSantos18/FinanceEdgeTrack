using Mapster;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Models.Abstract;

public abstract class Categoria
{
    public Guid CategoriaId { get; set; } = Guid.NewGuid();
    
    [Required(ErrorMessage = "É necessário um título para a categoria")]
    public string Titulo { get; set; } = default!;
    
    public string? Descricao { get; set; }

}
