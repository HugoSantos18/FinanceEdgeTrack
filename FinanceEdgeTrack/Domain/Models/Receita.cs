using FinanceEdgeTrack.Domain.Models.Abstract;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Domain.Models;

public class Receita : Categoria
{
    [Required(ErrorMessage = "É obrigatório informar o valor da receita.")]
    [Range(1, double.MaxValue)]
    public double Valor { get; set; }
    public DateTime Data { get; set; }
}
