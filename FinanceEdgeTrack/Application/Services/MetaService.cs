using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;

namespace FinanceEdgeTrack.Application.Services;

public class MetaService
{
    private readonly IMetaRepository _metaRepository;

    public MetaService(IMetaRepository metaRepository)
    {
        this._metaRepository = metaRepository;
    }
}
