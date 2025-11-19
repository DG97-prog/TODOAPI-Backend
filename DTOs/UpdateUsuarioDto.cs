namespace TodoApp.API.DTOs
{
    public class UpdateUsuarioDto
    {
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }          
        public string? Contrasena { get; set; }   
    }
}
