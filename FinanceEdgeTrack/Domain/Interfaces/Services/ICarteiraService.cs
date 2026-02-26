using FinanceEdgeTrack.Application.Dtos.Read;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface ICarteiraService
{
    Task<decimal> AdicionarSaldo(string UserId, CarteiraDTO saldo);
    Task<decimal> DescontarSaldo(string UserId, CarteiraDTO saldo);
}
