using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Write.Categorias;

public class CreateReceitaDTO
{
    public Guid CategoriaId { get; set; }

    [Required(ErrorMessage = "É necessário um título para o lançamento.")]
    public string Titulo { get; set; } = default!;

    public string? Descricao { get; set; }

    [Required(ErrorMessage = "É obrigatório informar o valor da receita.")]
    [Range(1,double.MaxValue)]
    public decimal Valor { get; set; }
    
    public DateTime Data { get; set; }
}
