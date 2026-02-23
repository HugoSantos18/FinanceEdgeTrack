using FinanceEdgeTrack.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Write.Categorias;

public class UpdateMetaDTO
{
    [Required(ErrorMessage = "É necessário um título para a Meta.")]
    public string Titulo { get; set; } = default!;

    [Range(1,99999999)]
    public decimal ValorAlvo { get; set; }
    
    public DateTime DataAlvo { get; set; }
    
    public Status Status { get; set; }

}
