using FinanceEdgeTrack.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinanceEdgeTrack.Domain.Models;

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

    public decimal ValorTotalAportes() => Aportes?.Sum(a => a.Valor) ?? 0;

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

    public void AdicionarAporte(AporteMetas aporte)
    {
        if (aporte is null)
            throw new ArgumentNullException(nameof(aporte));

        if (aporte.MetaId != MetaId)
            throw new InvalidOperationException("Aporte não pertence a esta meta.");

        Aportes ??= new List<AporteMetas>();
        Aportes.Add(aporte);
    }

    public void RemoverAporte(AporteMetas aporte)
    {
        if (aporte is null)
            throw new ArgumentNullException(nameof(aporte));

        Aportes.Remove(aporte);
    }

    public void AtualizarProgresso()
    {
        Aportes ??= new List<AporteMetas>();

        ValorAtual = Aportes.Sum(a => a.Valor);
        PorcentagemAtual = ValorAlvo == 0 ? 0 : (ValorAtual / ValorAlvo) * 100;
        ValorRestante = Math.Max(0, ValorAlvo - ValorAtual);

        var ultimo = Aportes.OrderByDescending(a => a.Data).FirstOrDefault();
        UltimoDepositoEmReais = ultimo?.Valor ?? 0;
        DataUltimoDeposito = ultimo?.Data ?? default;

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
