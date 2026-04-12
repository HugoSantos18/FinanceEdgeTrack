namespace FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Metas;

public class MetasKPIsGeralDTO
{
    public int MetasIniciadas { get; set; }
    public int TotalConcluidas { get; set; }
    public int TotalPendentes { get; set; }
    public int TotalCanceladas { get; set; }
    public decimal ValorTotalRestanteParaCompletarTodas { get; set; }
}
