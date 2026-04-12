namespace FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Consolidado;

public class DashboardConsolidadoDTO
{
    public decimal Saldo { get; set; }
    public decimal ValorTotalGasto { get; set; }
    public decimal ValorTotalGastosFixos { get; set; }
    public decimal ValorTotalRecebido { get; set; }
    public decimal ValorAlvoEmMetas { get; set; }
    public decimal ValorAportadoEmMetas { get; set; }
    public decimal PorcentagemConcluida { get; set; }
    public decimal ValorRestanteParaCompletarTodas { get; set; }
    public int TotalConcluidas { get; set; }
    public int TotalPendentes { get; set; }
    public int TotalCanceladas { get; set; }
}
