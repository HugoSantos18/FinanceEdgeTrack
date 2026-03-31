using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Write.Carteira;
using FinanceEdgeTrack.Application.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services.CarteiraService;
using FinanceEdgeTrack.Domain.Models;
using MapsterMapper;

namespace FinanceEdgeTrack.Application.Services.CarteiraService;

public class CarteiraService : ICarteiraService
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;
    private readonly ILogger<CarteiraService> _logger;
    private readonly CurrentUser _currentUser;

    public CarteiraService(IUnitOfWork uof, IMapper mapper, ILogger<CarteiraService> logger, CurrentUser currentUser)
    {
        _uof = uof;
        _mapper = mapper;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Carteira> CreateAsync(CreateCarteiraDTO carteiraDto)
    {
        var carteira = _mapper.Map<Carteira>(carteiraDto);

        await _uof.CarteiraRepository.CreateAsync(carteira);

        return carteira;
    }


    public async Task<ApiResponse<decimal>> AdicionarSaldoAsync(decimal valor)
    {
        if (valor <= 0)
            return ApiResponse<decimal>.Fail(ResultMessages.MoreThanZero);

        var carteira = await _uof.CarteiraRepository.GetAsync(u => u.UserId == _currentUser.UserId);

        if (carteira is null)
        {
            _logger.LogInformation($"Não foi possível adicionar o saldo, verifique os dados informados.");
            return ApiResponse<decimal>.Fail(ResultMessages.WalletNotFound);
        }

        var novoSaldo = carteira.Saldo += valor;

        await _uof.CarteiraRepository.UpdateAsync(carteira);
        await _uof.CommitAsync();

        return ApiResponse<decimal>.Ok(novoSaldo, $"Saldo adicionado com sucesso, novo saldo {novoSaldo:C2}");
    }

    public async Task<ApiResponse<decimal>> DescontarSaldoAsync(decimal valor)
    {
        var carteira = await _uof.CarteiraRepository.GetAsync(c => c.UserId == _currentUser.UserId);

        if (carteira is null)
        {
            _logger.LogInformation($"Não foi possível descontar o saldo, verifique os dados informados.");
            return ApiResponse<decimal>.Fail(ResultMessages.WalletNotFound);
        }

        if (valor <= 0)
        {
            _logger.LogInformation($"Não foi possível descontar o saldo, valor menor que zero.");
            return ApiResponse<decimal>.Fail(ResultMessages.MoreThanZero);
        }

        if (valor >= carteira.Saldo)
        {
            _logger.LogInformation($"Não foi possível descontar o saldo, valor maior que o saldo atual.");
            return ApiResponse<decimal>.Fail(ResultMessages.InvalidPrice);
        }

        var novoSaldo = carteira.Saldo -= valor;

        await _uof.CarteiraRepository.UpdateAsync(carteira);
        await _uof.CommitAsync();

        return ApiResponse<decimal>.Ok(novoSaldo, $"Valor descontado do saldo com sucesso, novo saldo: {novoSaldo:C2}");
    }

    public async Task<ApiResponse<decimal>> ObterSaldoAsync()
    {
        var carteira = await _uof.CarteiraRepository.GetAsync(c => c.UserId == _currentUser.UserId);

        if (carteira is null)
        {
            _logger.LogInformation($"Não foi possível obter o saldo, verifique o ID {_currentUser.UserId} do usuário informado.");
            return ApiResponse<decimal>.Fail(ResultMessages.WalletNotFound);
        }

        var saldoAtual = carteira.Saldo;

        return ApiResponse<decimal>.Ok(saldoAtual, $"Saldo atual: {saldoAtual}");
    }
}
