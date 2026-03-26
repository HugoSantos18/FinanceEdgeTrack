namespace FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Metas;

public class MetaDetalhadaResumoDTO
{
    public Guid MetaId { get; set; }
    public string? Titulo { get; set; }
    public decimal ValorAlvo { get; set; }
    public decimal ValorAportado { get; set; }
    public decimal Percentual { get; set; }
    public bool Concluida { get; set; }
}
