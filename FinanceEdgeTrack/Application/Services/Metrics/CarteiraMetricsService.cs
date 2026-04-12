using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Carteira;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using MapsterMapper;

namespace FinanceEdgeTrack.Application.Services.Metrics;

public class CarteiraMetricsService : ICarteiraMetrics
{
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uof;
    private readonly ILogger<CarteiraMetricsService> _logger;
    private readonly IMapper _mapper;

    public CarteiraMetricsService(ICurrentUser currentUser, ILogger<CarteiraMetricsService> logger,
                                  IUnitOfWork uof, IMapper mapper)
    {
        _currentUser = currentUser;
        _logger = logger;
        _uof = uof;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CarteiraResumoDTO>> GetSaldoAtualUser()
    {
        var carteira = await _uof.CarteiraRepository
                             .GetAsync(c => c.UserId.Equals(_currentUser.UserId));

        if (carteira == null)
        {
            _logger.LogError($"Não foi possível recuperar dados da carteira do usuário {_currentUser.UserId}", carteira);
            return ApiResponse<CarteiraResumoDTO>.Fail(ResultMessages.ErrorToGetWalletAmmountUser);
        }

        decimal saldoAtual = carteira.Saldo;

        var saldoDTO = _mapper.Map<CarteiraResumoDTO>(saldoAtual);

        return ApiResponse<CarteiraResumoDTO>.Ok(saldoDTO, $"Saldo atual: R${saldoDTO:C2}");
    }
}
