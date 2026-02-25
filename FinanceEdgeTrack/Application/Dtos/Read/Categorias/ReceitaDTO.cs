using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Read.Categorias;

public class ReceitaDTO : CategoriaBaseDTO
{
    [Required(ErrorMessage = "É obrigatório informar o valor da receita.")]
    [Range(1, 99999999)]
    public decimal Valor { get; set; }
    
    public DateTime Data { get; set; }
}
