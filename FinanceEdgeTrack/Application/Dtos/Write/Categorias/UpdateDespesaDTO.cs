namespace FinanceEdgeTrack.Application.Dtos.Write.Categorias
{
    public class UpdateDespesaDTO
    {
        public Guid CategoriaId { get; set; }
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public bool Fixa { get; set; }
    }
}
