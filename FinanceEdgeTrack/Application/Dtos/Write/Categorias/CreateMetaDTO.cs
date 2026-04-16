using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinanceEdgeTrack.Application.Dtos.Write.Categorias;

public class CreateMetaDTO
{
    public string? Descricao { get; set; }
    
    [Required(ErrorMessage = "É necessário ter um título para a Meta.")]
    public string Titulo { get; set; } = default!;
    
    [Required(ErrorMessage = "É obrigatório informar o valor da Meta para alcança-lá.")]
    [Range(typeof(decimal), "1", "99999999")]
    public decimal ValorAlvo { get; set; }

    public DateTime DataInicio { get; set; } = DateTime.UtcNow;
    
    public DateTime DataAlvo { get; set; }

    [JsonIgnore]
    public List<AporteMetasDTO>? Aportes { get; set; } 
}
