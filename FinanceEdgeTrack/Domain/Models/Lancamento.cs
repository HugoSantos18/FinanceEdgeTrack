using FinanceEdgeTrack.Domain.Models.Abstract;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FinanceEdgeTrack.Domain.Models;

[Table("Lancamento")]
public class Lancamento
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime DataLancamento { get; set; }

    public Guid CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public Categoria Categoria { get; set; } = default!;

    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; set; }  
}
