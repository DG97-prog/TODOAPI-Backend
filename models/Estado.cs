namespace TodoApp.API.Models
{
    public class Estado
    {
        public int Id { get; set; }

        public string NombreEstado { get; set; } = null!;

        // Relación: Un estado puede tener muchas tareas
        public ICollection<Tarea>? Tareas { get; set; }
    }
}
