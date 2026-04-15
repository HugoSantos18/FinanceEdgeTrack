using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Models;


[Table("AporteMetas")]
public class AporteMetas
{
    public Guid AporteMetasId { get;  set; }

    [Required(ErrorMessage = "É necessário informar um valor para o aporte.")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal Valor { get; set; }

    [Required]
    public Guid MetaId { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;
}
