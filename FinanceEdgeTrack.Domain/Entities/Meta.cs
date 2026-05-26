using FinanceEdgeTrack.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinanceEdgeTrack.Domain.Entities;

public class Meta
{
    [Key]
    public Guid MetaId { get; private set; } = Guid.NewGuid();

    public Guid CarteiraId { get; set; }

    [Required(ErrorMessage = "É necessário um título para a categoria")]
    public string Titulo { get; set; } = default!;

    public string? Descricao { get; set; }

    [JsonIgnore]
    public Carteira? Carteira { get; private set; }

    [Required(ErrorMessage = "É obrigatório informar o valor alvo da Meta para alcança-lá. :)")]
    [Range(typeof(decimal), "1", "999999999999")]
    public decimal ValorAlvo { get; set; }

    [Range(typeof(decimal), "0", "999999999999")]
    public decimal ValorAtual { get; private set; } = 0;

    [Range(typeof(decimal), "0", "999999999999")]
    public decimal UltimoDepositoEmReais { get; private set; }

    public DateTime DataUltimoDeposito { get; private set; }

    [Range(0, 100)]
    public decimal PorcentagemAtual { get; private set; }

    [Range(typeof(decimal), "0", "999999999999")]
    public decimal ValorRestante { get; private set; }

    public DateTime DataInicio { get; set; }

    public DateTime DataAlvo { get; set; }

    public DateTime? DataConclusao { get; private set; }

    public Status Status { get; private set; } = Status.EmAberto;

    public List<AporteMetas> Aportes { get; private set; } = new List<AporteMetas>();


    public decimal ValorRestanteParaCompletar() => ValorAlvo - ValorAtual;

    public void AlterarStatus(Status novoStatus)
    {
        if (Status == Status.Concluido)
            throw new InvalidOperationException("Meta já concluída");

        Status = novoStatus;
    }

    public DateTime AlterarDataAlvo(DateTime novaData)
    {
        if (novaData < DataInicio || novaData < DateTime.UtcNow)
            throw new InvalidOperationException("A meta deve ser alterada para uma data válida.");

        DataAlvo = novaData;
        return DataAlvo;
    }

    public void RecalcularProgresso(decimal novoTotal, decimal ultimoValor, DateTime ultimaData)
    {
        ValorAtual = novoTotal;
        PorcentagemAtual = ValorAlvo == 0 ? 0 : (ValorAtual / ValorAlvo) * 100;
        ValorRestante = Math.Max(0, ValorAlvo - ValorAtual);
        UltimoDepositoEmReais = ultimoValor;
        DataUltimoDeposito = ultimaData;

        if (ValorAtual >= ValorAlvo && Status != Status.Concluido)
            Finalizar();
    }

    private void Finalizar()
    {
        Status = Status.Concluido;
        PorcentagemAtual = 100;
        ValorRestante = 0;
        DataConclusao = DateTime.UtcNow;
    }
}
