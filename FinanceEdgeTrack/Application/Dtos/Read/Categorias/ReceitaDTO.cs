using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Categorias;

public class ReceitaDTO : CategoriaBaseDTO
{
    [Required(ErrorMessage = "É obrigatório informar o valor da receita.")]
    [Range(1, double.MaxValue)]
    public double Valor { get; set; }
    
    public DateTime Data { get; set; }
}
