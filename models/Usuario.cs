namespace TodoApp.API.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string NombreUsuario { get; set; } = null!;

        public string ContrasenaHash { get; set; } = null!;

        public string Correo { get; set; } = null!;

        public string Rol { get; set; } = "User";

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relación: Un usuario puede tener muchas tareas
        public ICollection<Tarea>? Tareas { get; set; }
    }
}
