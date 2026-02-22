using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceEdgeTrack.Domain.Models.Abstract;

namespace FinanceEdgeTrack.Domain.Models;

public class Despesa : Categoria
{
    [Required(ErrorMessage = "É obrigatório informar o valor da despesa.")]
    [Range(1, double.MaxValue)]
    public decimal Valor { get; set; }

    public bool Fixa { get; set; }
    
    public DateTime Data { get; set; }

}
