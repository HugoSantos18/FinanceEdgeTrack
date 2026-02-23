using FinanceEdgeTrack.Domain.Models.Abstract;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Domain.Models;

public class Receita : Categoria
{
    [Required(ErrorMessage = "É obrigatório informar o valor da receita.")]
    [Range(typeof(decimal), "1", "999999999999")]
    public double Valor { get; set; }
    public DateTime Data { get; set; }
}
