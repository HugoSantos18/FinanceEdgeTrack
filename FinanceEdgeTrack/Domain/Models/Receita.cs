using FinanceEdgeTrack.Domain.Models.Abstract;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Domain.Models;

public class Receita : Categoria
{
    public double Valor { get; set; }
    public DateTime Data { get; set; }
}
