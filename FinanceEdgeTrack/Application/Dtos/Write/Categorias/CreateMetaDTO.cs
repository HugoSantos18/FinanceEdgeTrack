using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Domain.Enum;
using FinanceEdgeTrack.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Write.Categorias;

public class CreateMetaDTO
{
    public Guid CategoriaId { get; set; }
    
    public string? Descricao { get; set; }
    
    [Required(ErrorMessage = "É necessário ter um título para a Meta.")]
    public string Titulo { get; set; } = default!;
    
    [Required(ErrorMessage = "É obrigatório informar o valor da Meta para alcança-lá.")]
    [Range(1, double.MaxValue)]
    public decimal ValorAlvo { get; set; }
    
    public DateTime DataInicio { get; set; }
    
    public DateTime DataAlvo { get; set; }
    
}
