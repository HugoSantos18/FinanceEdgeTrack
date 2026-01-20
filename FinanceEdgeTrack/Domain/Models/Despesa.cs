using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceEdgeTrack.Domain.Models.Abstract;

namespace FinanceEdgeTrack.Domain.Models;

public class Despesa : Categoria
{
    public decimal Valor { get; set; }
    public bool Fixa { get; set; }
    public DateTime Data { get; set; }

}
