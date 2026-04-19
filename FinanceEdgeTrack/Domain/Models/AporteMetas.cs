using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Models;


[Table("AporteMetas")]
public class AporteMetas
{
    [Key]
    public Guid AporteMetasId { get; private set; } = Guid.NewGuid();

    [Required(ErrorMessage = "É necessário informar um valor para o aporte.")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal Valor { get; set; }

    public DateTime Data { get; set; } = DateTime.UtcNow;

    private AporteMetas() { }

    public static AporteMetas Criar(decimal valor)
    {
        return new AporteMetas()
        {
            Valor = valor,
            Data = DateTime.UtcNow
        };
    }
}
