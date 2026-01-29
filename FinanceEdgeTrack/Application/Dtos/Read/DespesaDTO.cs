using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read;

public class DespesaDTO : CategoriaBaseDTO
{
    [Required(ErrorMessage = "É obrigatório informar o valor da despesa.")]
    [Range(1, double.MaxValue)]
    public decimal Valor { get; set; }
    
    public bool Fixa { get; set; }
    
    public DateTime Data { get; set; }
}
