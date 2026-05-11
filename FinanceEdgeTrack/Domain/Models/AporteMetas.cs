using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceEdgeTrack.Domain.Models;


[Table("AporteMetas")]
public class AporteMetas
{
    [Key]
    public Guid AporteMetasId { get; private set; } = Guid.NewGuid();

    [Required]
    public Guid MetaId { get; private set; }

    [Required(ErrorMessage = "É necessário informar um valor para o aporte.")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal Valor { get; private set; }

    public DateTime Data { get; private set; } = DateTime.UtcNow;

    private AporteMetas() { }

    public static AporteMetas Criar(Guid metaId, decimal valor)
    {
        if (metaId == Guid.Empty)
            throw new ArgumentException("MetaId inválido.", nameof(metaId));

        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero.", nameof(valor));

        return new AporteMetas
        {
            MetaId = metaId,
            Valor = valor,
            Data = DateTime.UtcNow
        };
    }
}
