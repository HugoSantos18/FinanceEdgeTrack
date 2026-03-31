namespace FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Consolidado;

public class DashboardConsolidadoDTO
{
    public decimal Saldo { get; set; }
    public decimal MediaGastosDiario { get; set; }
    public decimal ValorTotalGasto { get; set; }
    public decimal ValorTotalRecebido { get; set; }
    public int TotalConcluidas { get; set; }
    public int TotalPendentes { get; set; }
    public int TotalCanceladas { get; set; }
}
