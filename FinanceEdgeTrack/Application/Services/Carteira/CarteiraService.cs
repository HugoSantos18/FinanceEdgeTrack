using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.CarteiraService;
using FinanceEdgeTrack.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.CarteiraService;

public class CarteiraService : ICarteiraService
{
    private readonly IUnitOfWork _uof;
    private readonly ILogger<CarteiraService> _logger;
    private readonly ICurrentUser _currentUser;

    public CarteiraService(IUnitOfWork uof, ILogger<CarteiraService> logger, ICurrentUser currentUser)
    {
        _uof = uof;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Carteira> GetCarteiraAsync()
    {
        var carteira = await _uof.CarteiraRepository
                         .Query()
                         .FirstOrDefaultAsync(c => c.UserId.Equals(_currentUser.UserId));

        return carteira ?? throw new InvalidOperationException(ResultMessages.WalletNotFound);
    }

    public async Task<Carteira> CreateAsync()
    {
        var userId = _currentUser.UserId;

        var exists = await GetCarteiraAsync();

        if (exists is not null)
        {
            _logger.LogWarning($"Usuário já possui uma carteira de ID: {exists.CarteiraId}.");
            throw new InvalidOperationException("Usuário já possui uma carteira.");
        }

        var carteira = Carteira.CriarCarteira(userId.ToString());
        _logger.LogInformation($"User encontrado com sucesso: {userId}");

        await _uof.CarteiraRepository.CreateAsync(carteira);
        await _uof.CommitAsync();

        return carteira;
    }


    public async Task<ApiResponse<decimal>> AdicionarSaldoAsync(decimal valor)
    {
        if (valor <= 0)
            return ApiResponse<decimal>.Fail(ResultMessages.MoreThanZero);

        var carteira = await GetCarteiraAsync();

        carteira.AdicionarSaldo(valor);

        await _uof.CommitAsync();

        return ApiResponse<decimal>.Ok(carteira.Saldo, $"Saldo adicionado com sucesso, novo saldo R${carteira.Saldo:C2}");
    }

    public async Task<ApiResponse<decimal>> DescontarSaldoAsync(decimal valor)
    {
        if (valor <= 0)
            return ApiResponse<decimal>.Fail(ResultMessages.MoreThanZero);

        var carteira = await GetCarteiraAsync();

        if (valor > carteira.Saldo)
        {
            _logger.LogInformation($"Valor de retirada R${valor:C2} maior que saldo atual: R${carteira.Saldo:C2}");
            return ApiResponse<decimal>.Fail(ResultMessages.InvalidPrice + $"\nSeu saldo atual: R${carteira.Saldo:C2}");
        }

        carteira.DescontarSaldo(valor);
        
        await _uof.CommitAsync();

        return ApiResponse<decimal>.Ok(carteira.Saldo, $"Valor descontado do saldo com sucesso, novo saldo: {carteira.Saldo:C2}");
    }

    public async Task<ApiResponse<decimal>> ObterSaldoAsync()
    {
        var carteira = await GetCarteiraAsync();

        return ApiResponse<decimal>.Ok(carteira.Saldo, $"Saldo atual: {carteira.Saldo}");
    }

}
