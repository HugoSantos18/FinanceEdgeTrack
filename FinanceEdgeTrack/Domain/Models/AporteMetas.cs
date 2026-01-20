using System.ComponentModel.DataAnnotations.Schema;
using FinanceEdgeTrack.Domain.Models;

namespace FinanceEdgeTrack.Domain.Models;


[Table("AporteMetas")]
public class AporteMetas
{
    public Guid Id { get; set; }    
    
    public decimal Valor { get; set; }

    public Guid MetaId { get; set; }

    [ForeignKey(nameof(MetaId))]
    public Meta Meta { get; set; } = default!;

}
