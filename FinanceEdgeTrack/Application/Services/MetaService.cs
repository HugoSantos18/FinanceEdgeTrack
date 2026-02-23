using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Interfaces;

namespace FinanceEdgeTrack.Application.Services;

public class MetaService
{
    private readonly IMeta _meta;
    private readonly IMetaRepository _metaRepository;

   public MetaService(IMeta meta, IMetaRepository metaRepository)
    {
        this._meta = meta;  
        this._metaRepository = metaRepository; 
    }
}
