using FinanceEdgeTrack.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Write.Categorias;

public class CreateMetaDTO
{
    [Required(ErrorMessage = "É necessário ter um título para a Meta.")]
    public string Titulo { get; set; } = default!;
    
    [Required(ErrorMessage = "É obrigatório informar o valor da Meta para alcança-lá.")]
    [Range(1, double.MaxValue)]
    public decimal ValorAlvo { get; set; }
    
    public DateTime DataInicio { get; set; }
    
    public DateTime DataAlvo { get; set; }
    
    public Status Status { get; set; } = default!;
}
