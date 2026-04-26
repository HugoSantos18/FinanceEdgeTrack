using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Write.Categorias;

public class CreateDespesaDTO
{

    [Required(ErrorMessage = "É necessário um título para a despesa que está sendo colocada.")]
    public string Titulo { get; set; } = default!;

    public string? Descricao { get; set; }

    [Required(ErrorMessage = "É obrigatório informar o valor da despesa.")]
    [Range(1, double.MaxValue)]
    public decimal Valor { get; set; }

    public DateTime Data { get; set; } = DateTime.UtcNow;

    public bool Fixa { get; set; } = default!;
}
