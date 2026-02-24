using FinanceEdgeTrack.Domain.Enum;
using FinanceEdgeTrack.Domain.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using FinanceEdgeTrack.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Domain.Models;

public class Meta : Categoria
{

    [Required(ErrorMessage = "É obrigatório informar o valor alvo da Meta para alcança-lá. :)")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal ValorAlvo { get; set; }

    [Range(typeof(decimal), "0", "999999999999")]
    public decimal ValorAtual { get; set; } = 0;

    [Range(typeof(decimal), "1", "999999999999")]
    public decimal UltimoDepositoEmReais { get; set; } = default!;

    public DateTime DataUltimoDeposito { get; set; }

    [Range(0, 100)]
    public decimal PorcentagemAtual { get; set; }

    [Range(typeof(decimal), "0", "999999999999")]
    public decimal ValorRestante { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime DataAlvo { get; set; }

    public Status Status { get; set; } = default!;

    public List<AporteMetas>? Aportes { get; set; } = new();


    public void AlterarStatus(Status novoStatus)
    {
        if (Status == Status.Concluido)
            throw new InvalidOperationException("Meta já concluída");

        Status = novoStatus;
    }

    public DateTime AlterarDataAlvo(DateTime novaData)
    {
        if (novaData < DataInicio || novaData < DateTime.UtcNow)
        {
            throw new InvalidOperationException("A meta deve ser alterada para uma data válida.");
        }

        DataAlvo = novaData;

        return DataAlvo;
    }

    public void RegistrarAporte(AporteMetas novoAporte)
    {
        if (novoAporte is null)
            throw new ArgumentNullException(nameof(novoAporte));


        if (novoAporte.Valor <= 0)
            throw new InvalidOperationException("Valor inválido");

        Aportes ??= new List<AporteMetas>();
        Aportes.Add(novoAporte);

        AtualizarHistoricoAporte(novoAporte.Valor);
        AdicionareAtualizarValorAporte(novoAporte.Valor);
        PorcentagemAtual = CalcularPorcentagemAtual();
    }

    private void AdicionareAtualizarValorAporte(decimal novoValor)
    {
        ValorAtual += novoValor;
        ValorRestante = ValorAlvo - ValorAtual;
    }

    public void AtualizarHistoricoAporte(decimal novoValor)
    {
        DataUltimoDeposito = DateTime.UtcNow;
        UltimoDepositoEmReais = novoValor;
    }

    public decimal CalcularPorcentagemAtual()
    {
        if (ValorAlvo == 0)
            return 0;

        return (ValorAtual / ValorAlvo) * 100;
    }

    public void FinalizarMeta()
    {
        if (ValorAtual >= ValorAlvo)
        {
            AlterarStatus(Status.Concluido);
            PorcentagemAtual = 100;
            ValorRestante = 0;
        }
        else
            throw new InvalidOperationException("Meta ainda não finalizada, complete o valor alvo para poder finalizar.");
    }
}
