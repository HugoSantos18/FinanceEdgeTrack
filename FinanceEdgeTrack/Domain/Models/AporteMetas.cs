using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Models;


[Table("AporteMetas")]
public class AporteMetas
{
    public Guid Id { get; private set; }

    [Required(ErrorMessage = "É necessário informar um valor para o aporte.")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal Valor { get; private set; }

    [Required]
    public Guid MetaId { get; private set; }

    [ForeignKey(nameof(MetaId))]
    public Meta Meta { get; private set; } = default!;

}
