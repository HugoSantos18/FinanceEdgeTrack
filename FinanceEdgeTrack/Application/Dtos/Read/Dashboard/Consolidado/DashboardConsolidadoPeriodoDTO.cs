namespace FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Consolidado;

public class DashboardConsolidadoPeriodoDTO
{
    public decimal Saldo { get; set; }
    public decimal MediaGastosDiarioNoPeriodo { get; set; }
    public decimal ValorTotalGastoNoPeriodo { get; set; }
    public decimal ValorTotalRecebidoNoPeriodo { get; set; }
    public decimal ValorAlvoEmMetasNoPeriodo { get; set; }
    public decimal ValorAportadoEmMetasNoPeriodo { get; set; }
    public int TotalConcluidasNoPeriodo { get; set; }
    public int TotalPendentesNoPeriodo { get; set; }
    public int TotalCanceladasNoPeriodo { get; set; }
}
