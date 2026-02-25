using FinanceEdgeTrack.Domain.Models.Abstract;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FinanceEdgeTrack.Domain.Models;

[Table("Lancamento")]
public class Lancamento
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public DateTime DataLancamento { get; set; }

    [Required]
    public Guid CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public Categoria Categoria { get; set; } = default!;

    [Required]
    public string? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }  

}
