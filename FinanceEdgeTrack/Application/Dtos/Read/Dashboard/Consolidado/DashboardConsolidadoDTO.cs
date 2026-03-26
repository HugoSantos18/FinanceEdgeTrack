namespace FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Consolidado;

public class DashboardConsolidadoDTO
{
    public decimal MediaGastosDiario { get; set; }
    public decimal ValorTotalGasto { get; set; }
    public decimal ValorTotalRecebido { get; set; }

    // propriedades dos mensais para consolidar em um unico DashboardConsolidadoMensalDTO
    // MetasKPIsMensal, MetasResumoMensal, ReceitaResumoMensal, DespesaResumoMensal, CarteiraResumoMensal.
}
