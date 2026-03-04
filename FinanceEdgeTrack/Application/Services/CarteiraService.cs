using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Write.Carteira;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Error;
using MapsterMapper;
using System.Runtime.InteropServices;

namespace FinanceEdgeTrack.Application.Services;

public class CarteiraService : ICarteiraService
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public CarteiraService(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    public async Task<Carteira> CreateAsync(CreateCarteiraDTO carteiraDto)
    {
        var carteira = _mapper.Map<Carteira>(carteiraDto);

        await _uof.CarteiraRepository.CreateAsync(carteira);

        return carteira;
    }


    public async Task AdicionarSaldoAsync(string userId, decimal valor)
    {
        if(valor <= 0)
        {
            throw new InvalidOperationException(ResultMessages.MoreThanZero);
        }
        
        var carteira = await _uof.CarteiraRepository.GetAsync(u => u.UserId == userId);

        if (carteira is null)
            throw new KeyNotFoundException(ResultMessages.NotFoundUser);

        carteira.Saldo += valor;

        await _uof.CarteiraRepository.UpdateAsync(carteira);
        await _uof.CommitAsync();
    }

    public async Task DescontarSaldoAsync(string userId, decimal valor)
    {
        var carteira = await _uof.CarteiraRepository.GetAsync(c => c.UserId == userId);

        if (carteira is null)
            throw new KeyNotFoundException(ResultMessages.NotFoundUser);

        if (valor <= 0)
            throw new InvalidOperationException(ResultMessages.MoreThanZero);

        if (valor >= carteira.Saldo)
            throw new InvalidOperationException(ResultMessages.InvalidPrice);

        carteira.Saldo -= valor;

        await _uof.CarteiraRepository.UpdateAsync(carteira);
        await _uof.CommitAsync();
    }

    public async Task<decimal> ObterSaldoAsync(string userId)
    {
        var carteira = await _uof.CarteiraRepository.GetAsync(c => c.UserId == userId);

        if (carteira is null)
            throw new KeyNotFoundException(ResultMessages.WalletNotFound);

        return carteira.Saldo;
    }
}
