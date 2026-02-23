using FinanceEdgeTrack.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Controllers;

[ApiController]
[Route("[controller]")]
public class MetaController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public MetaController(IUnitOfWork uof)
    {
        this._uof = uof;
    }

    // método PATCH para alterar o valor da meta associado ou deixar no PUT (verificar depois qual é o melhor)



}
