namespace TodoApp.API.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relación: Una categoría puede tener muchas tareas
        public ICollection<Tarea>? Tareas { get; set; }
    }
}
