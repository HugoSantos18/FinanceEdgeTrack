using FinanceEdgeTrack.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace FinanceEdgeTrack.Domain.Validations;

public class OptionalCategoryAtributte : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext _validationContext)
    {
        var lancamento = value as Lancamento;

        if ((lancamento.ReceitaId is null && lancamento.DespesaId is null) ||
    (lancamento.ReceitaId is not null && lancamento.DespesaId is not null))
        {
            return new ValidationResult("O lançamento deve estar associado a uma receita ou uma despesa.");
        }

        if(lancamento is null)
        {
            return ValidationResult.Success;
        }

        return ValidationResult.Success;
    }

}
