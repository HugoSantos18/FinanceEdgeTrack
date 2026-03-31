using FinanceEdgeTrack.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using FinanceEdgeTrack.Application.Common.Responses;

namespace FinanceEdgeTrack.Domain.Models;

public class Meta
{
    public Guid MetaId { get; private set; } = Guid.NewGuid();

    [Required(ErrorMessage = "É necessário um título para a categoria")]
    public string Titulo { get; set; } = default!;

    public string? Descricao { get; private set; }

    [Required(ErrorMessage = "É obrigatório informar o valor alvo da Meta para alcança-lá. :)")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal ValorAlvo { get; set; }

    [Range(typeof(decimal), "0", "999999999999")]
    public decimal ValorAtual { get; private set; } = 0;

    [Range(typeof(decimal), "1", "999999999999")]
    public decimal UltimoDepositoEmReais { get; private set; } = default!;

    public DateTime DataUltimoDeposito { get; private set; }

    [Range(0, 100)]
    public decimal PorcentagemAtual { get; private set; }

    [Range(typeof(decimal), "0", "999999999999")]
    public decimal ValorRestante { get; private set; }

    public DateTime DataInicio { get; private set; }

    public DateTime DataAlvo { get; private set; }

    public DateTime? DataConclusao { get; private set; }

    public Status Status { get; private set; } = default!;

    public List<AporteMetas>? Aportes { get; private set; } = new();


    public decimal ValorRestanteParaCompletar()
    {
        ValorRestante = ValorAlvo - ValorAtual;
        return ValorRestante;
    }

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

        if (ValorTotalAportes() >= ValorAlvo)
        {
            FinalizarMeta();
        }

        AdicionareAtualizarValorAporte(novoAporte.Valor);
    }

    public void RemoverAporte(AporteMetas aporteRemovido)
    {
        if (aporteRemovido is null)
            throw new ArgumentNullException(nameof(aporteRemovido));

        int aporteAnterior = (Aportes.IndexOf(aporteRemovido) - 1);
        Aportes.Remove(aporteRemovido);

        DescontareAtualizarValorAporte(aporteRemovido.Valor);
        AtualizarHistoricoAporteRemoved(aporteAnterior);
    }

    private void AdicionareAtualizarValorAporte(decimal novoValor)
    {
        ValorAtual += novoValor;
        ValorRestante = ValorAlvo - ValorAtual;
        UltimoDepositoEmReais = novoValor;
        CalcularPorcentagemAtual();
        DataUltimoDeposito = DateTime.UtcNow;
    }

    private void DescontareAtualizarValorAporte(decimal valorRemovido)
    {
        ValorAtual -= valorRemovido;
        ValorRestante = ValorAlvo - (ValorAtual - valorRemovido);
        CalcularPorcentagemAtual();
    }


    private void AtualizarHistoricoAporteRemoved(int index)
    {
        if (index < 0 || index > Aportes.Count)
            throw new InvalidOperationException(ResultMessages.InvalidCredentials);

        DataUltimoDeposito = DateTime.UtcNow;
        UltimoDepositoEmReais = Aportes[index].Valor;

    }

    private decimal CalcularPorcentagemAtual()
    {
        if (ValorAlvo == 0)
            return 0;

        return (ValorAtual / ValorAlvo) * 100;
    }

    private void FinalizarMeta()
    {
        AlterarStatus(Status.Concluido);
        PorcentagemAtual = 100;
        ValorRestante = 0;
        DataConclusao = DateTime.UtcNow;
    }


    public decimal ValorTotalAportes()
    {
        decimal total = 0;

        total = Aportes.Sum(a => a.Valor);

        return total;
    }
}
