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
        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        return await GetByUserIdAsync(userId);
    }

    public async Task<Carteira> GetByUserIdAsync(string userId)
    {
        return await _uof.CarteiraRepository
            .Query()
            .FirstOrDefaultAsync(c => c.UserId == userId);

    }

    public async Task<Carteira> CreateAsync(string userId)
    {
        var exists = await _uof.CarteiraRepository
                               .Query()
                               .FirstOrDefaultAsync(c => c.UserId == userId);

        if (exists is not null)
        {
            _logger.LogWarning($"Usuário já possui uma carteira de ID: {exists.CarteiraId}.");
            throw new InvalidOperationException("Usuário já possui uma carteira.");
        }

        var carteira = Carteira.CriarCarteira(userId);
        _logger.LogInformation($"Criadno carteira para User: {userId}");

        await _uof.CarteiraRepository.CreateAsync(carteira);
        await _uof.CommitAsync();

        return carteira;
    }


    public async Task<bool> DebitarSaldoComGuardaAsync(Guid carteiraId, decimal valor)
    {
        if (valor <= 0)
            return false;

        var rows = await _uof.CarteiraRepository
            .Query()
            .Where(c => c.CarteiraId == carteiraId && c.Saldo >= valor)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Saldo, c => c.Saldo - valor));

        return rows == 1;
    }

    public async Task CreditarSaldoAsync(Guid carteiraId, decimal valor)
    {
        if (valor <= 0)
            return;

        await _uof.CarteiraRepository
            .Query()
            .Where(c => c.CarteiraId == carteiraId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Saldo, c => c.Saldo + valor));
    }
}
