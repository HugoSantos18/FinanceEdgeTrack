using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Error;
using System.Runtime.InteropServices;

namespace FinanceEdgeTrack.Application.Services;

public class CarteiraService : ICarteiraService
{
    private readonly IUnitOfWork _uof;

    public CarteiraService(IUnitOfWork uof)
    {
        this._uof = uof;
    }

    public async Task AdicionarSaldoAsync(string userId, decimal valor)
    {
        if(valor <= 0)
        {
            throw new InvalidOperationException(ErrorMessages.MoreThanZero);
        }
        
        var carteira = await _uof.CarteiraRepository.Get(u => u.UserId == userId);

        if (carteira is null)
            throw new KeyNotFoundException(ErrorMessages.NotFoundUser);

        carteira.Saldo += valor;

        await _uof.CarteiraRepository.Update(carteira);
        await _uof.CommitAsync();
    }

    public async Task DescontarSaldoAsync(string userId, decimal valor)
    {
        var carteira = await _uof.CarteiraRepository.Get(c => c.UserId == userId);

        if (carteira is null)
            throw new KeyNotFoundException(ErrorMessages.NotFoundUser);

        if (valor <= 0)
            throw new InvalidOperationException(ErrorMessages.MoreThanZero);

        if (valor >= carteira.Saldo)
            throw new InvalidOperationException(ErrorMessages.InvalidPrice);

        carteira.Saldo -= valor;

        await _uof.CarteiraRepository.Update(carteira);
        await _uof.CommitAsync();
    }

    public async Task<decimal> ObterSaldoAsync(string userId)
    {
        var carteira = await _uof.CarteiraRepository.Get(c => c.UserId == userId);

        if (carteira is null)
            throw new KeyNotFoundException(ErrorMessages.WalletNotFound);

        return carteira.Saldo;
    }
}
