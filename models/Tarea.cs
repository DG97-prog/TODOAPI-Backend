namespace TodoApp.API.Models
{
    public class Tarea
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }

        public int? CategoriaId { get; set; }

        public int? UsuarioId { get; set; }

        public int? EstadoId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        // Relaciones
        public Categoria? Categoria { get; set; }

        public Usuario? Usuario { get; set; }

        public Estado? Estado { get; set; }
    }
}
