namespace FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Metas;

public class MetasKPIsDTO
{
    public int TotalConcluidasNoMes { get; set; }
    public int TotalPendentesNoMes { get; set; }
    public int TotalCanceladasNoMes { get; set; }
    public decimal ValorTotalRestanteParaCompletar { get; set; }
    public int MediaDiasParaCompletar { get; set; }
}
