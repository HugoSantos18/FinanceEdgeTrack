using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Carteira;
using FinanceEdgeTrack.Application.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using MapsterMapper;

namespace FinanceEdgeTrack.Application.Services.Metrics;

public class CarteiraMetricsService : ICarteiraMetrics
{
    private readonly CurrentUser _currentUser;
    private readonly IUnitOfWork _uof;
    private readonly ILogger<CarteiraMetricsService> _logger;
    private readonly IMapper _mapper;

    public CarteiraMetricsService(CurrentUser currentUser, ILogger<CarteiraMetricsService> logger,
                                  IUnitOfWork uof, IMapper mapper)
    {
        _currentUser = currentUser;
        _logger = logger;
        _uof = uof;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CarteiraResumoDTO>> GetSaldoAtualUser()
    {
        var carteira = await _uof.CarteiraRepository.GetAsync(c => c.UserId == _currentUser.UserId);

        if (carteira == null)
        {
            _logger.LogInformation($"Não foi possível recuperar dados da carteira do usuário");
            return ApiResponse<CarteiraResumoDTO>.Fail(ResultMessages.ErrorToGetWalletAmmountUser);
        }

        decimal saldoAtual = carteira.Saldo;

        var saldoDTO = _mapper.Map<CarteiraResumoDTO>(saldoAtual);

        return ApiResponse<CarteiraResumoDTO>.Ok(saldoDTO, $"Saldo atual: R${saldoDTO:C2}");
    }
}
