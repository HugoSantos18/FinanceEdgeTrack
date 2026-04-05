namespace FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Metas;

public class MetasKPIsPeriodoDTO
{
    public int MetasIniciadasNoPeriodo { get; set; }
    public int TotalConcluidasNoPeriodo { get; set; }
    public int TotalPendentesNoPeriodo { get; set; }
    public int TotalCanceladasNoPeriodo { get; set; }
    public decimal ValorTotalRestanteParaCompletar { get; set; }
    public double MediaDiasParaCompletar { get; set; }
}
