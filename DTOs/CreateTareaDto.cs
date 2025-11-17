namespace TodoApp.API.DTOs
{
    public class CreateTareaDto
    {
        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int? CategoriaId { get; set; }
        public int? EstadoId { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}
