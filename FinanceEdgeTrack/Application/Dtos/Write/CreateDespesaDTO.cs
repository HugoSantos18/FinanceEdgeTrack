using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Application.Dtos.Write;

public class CreateDespesaDTO
{
    [Required(ErrorMessage = "É necessário um título para a despesa que está sendo colocada.")]
    public string Titulo { get; set; } = default!;

    [Required(ErrorMessage = "É obrigatório informar o valor da despesa.")]
    [Range(1, double.MaxValue)]
    public decimal Valor { get; set; }

    public DateTime Data { get; set; }

    public bool Fixa { get; set; } = default!;
}
