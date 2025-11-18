namespace TodoApp.API.DTOs
{
    public class RegisterDto
    {
        public string NombreUsuario { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Rol { get; set; } = "User";
    }
}
