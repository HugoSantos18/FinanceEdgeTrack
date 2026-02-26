using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using MapsterMapper;
using FinanceEdgeTrack.Domain.Interfaces;

namespace FinanceEdgeTrack.Application.Services;

public class MetaService : IMetaService
{
    private readonly IMetaRepository _metaRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uof;

    public MetaService(IMetaRepository metaRepository, IMapper mapper, IUnitOfWork uof)
    {
        this._mapper = mapper;
        this._metaRepository = metaRepository;
        this._uof = uof;
    }


}
