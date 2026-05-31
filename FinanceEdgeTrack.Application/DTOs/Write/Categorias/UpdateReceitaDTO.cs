using System.Text.Json.Serialization;

namespace FinanceEdgeTrack.Application.DTOs.Write.Categorias
{
    public class UpdateReceitaDTO
    {
        [JsonIgnore]
        public Guid ReceitaId { get; set; }
        public string Titulo { get; set; } = default!;
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;  
    }
}
