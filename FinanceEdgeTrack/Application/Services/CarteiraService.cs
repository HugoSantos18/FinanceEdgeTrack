using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using System.Runtime.InteropServices;

namespace FinanceEdgeTrack.Application.Services;

public class CarteiraService : ICarteiraService
{
    private readonly IUnitOfWork _uof; // add UserRepository para conseguir buscar um User por ID.
     
    public CarteiraService(IUnitOfWork uof)
    {
        this._uof = uof;
    }

    // fazer implementação após implementar a autenticação, assim podendo utilizar os dados/claims para utilização da carteira.

    public async Task<decimal> AdicionarSaldo(string UserId, CarteiraDTO saldo)
    {
        throw new NotImplementedException();
    }

    public async Task<decimal> DescontarSaldo(string UserId, CarteiraDTO saldo)
    {
        throw new NotImplementedException();
    }
}
