namespace FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Consolidado;

public class DashboardConsolidadoNoMesDTO
{
    public decimal Saldo { get; set; }
    public decimal MediaGastosDiarioNoMes { get; set; }
    public decimal ValorTotalGastoNoMes { get; set; }
    public decimal ValorTotalRecebidoNoMes { get; set; }
    public int TotalConcluidasNoMes { get; set; }
    public int TotalPendentesNoMes { get; set; }
    public int TotalCanceladasNoMes { get; set; }
}
