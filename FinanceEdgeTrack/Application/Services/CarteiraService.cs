using FinanceEdgeTrack.Application.Common;
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


    public async Task<ApiResponse<decimal>> AdicionarSaldoAsync(string userId, decimal valor)
    {
        if (valor <= 0)
            return ApiResponse<decimal>.Fail(ResultMessages.MoreThanZero);

        var carteira = await _uof.CarteiraRepository.GetAsync(u => u.UserId == userId);

        if (carteira is null)
            return ApiResponse<decimal>.Fail(ResultMessages.WalletNotFound);

        var novoSaldo = (carteira.Saldo += valor);

        await _uof.CarteiraRepository.UpdateAsync(carteira);
        await _uof.CommitAsync();

        return ApiResponse<decimal>.Ok(novoSaldo, $"Saldo adicionado com sucesso, novo saldo {novoSaldo:C2}");
    }

    public async Task<ApiResponse<decimal>> DescontarSaldoAsync(string userId, decimal valor)
    {
        var carteira = await _uof.CarteiraRepository.GetAsync(c => c.UserId == userId);

        if (carteira is null)
            return ApiResponse<decimal>.Fail(ResultMessages.WalletNotFound);

        if (valor <= 0)
            return ApiResponse<decimal>.Fail(ResultMessages.MoreThanZero);

        if (valor >= carteira.Saldo)
            return ApiResponse<decimal>.Fail(ResultMessages.InvalidPrice);

        var novoSaldo = (carteira.Saldo -= valor);

        await _uof.CarteiraRepository.UpdateAsync(carteira);
        await _uof.CommitAsync();

        return ApiResponse<decimal>.Ok(novoSaldo, $"Valor descontado do saldo com sucesso, novo saldo: {novoSaldo:C2}");
    }

    public async Task<ApiResponse<decimal>> ObterSaldoAsync(string userId)
    {
        var carteira = await _uof.CarteiraRepository.GetAsync(c => c.UserId == userId);

        if (carteira is null)
            return ApiResponse<decimal>.Fail(ResultMessages.WalletNotFound);

        var saldoAtual = carteira.Saldo;

        return ApiResponse<decimal>.Ok(saldoAtual, $"Saldo atual: {saldoAtual}");
    }
}
