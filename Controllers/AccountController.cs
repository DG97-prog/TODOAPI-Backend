using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TodoApp.API.Data;
using TodoApp.API.DTOs;
using TodoApp.API.Interfaces;
using TodoApp.API.Models;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AccountController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var usuario = await _authService.RegistrarAsync(registerDto);
                return Ok(new { mensaje = "Usuario registrado exitosamente", usuarioId = usuario.Id });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto);
            if (token == null)
                return Unauthorized(new { mensaje = "Nombre de usuario o contraseña incorrectos." });

            return Ok(new { token });
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new
                {
                    u.Id,
                    u.NombreUsuario,
                    u.Correo,
                    u.Rol,
                    u.FechaCreacion
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUsuarioDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            var existeNombre = await _context.Usuarios.AnyAsync(u =>
                u.NombreUsuario == dto.NombreUsuario && u.Id != id);

            if (existeNombre)
                return BadRequest(new { mensaje = "El nombre de usuario ya está en uso." });

            usuario.NombreUsuario = dto.NombreUsuario;
            usuario.Correo = dto.Correo;
            usuario.Rol = dto.Rol;

            if (!string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            }

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                usuario.Id,
                usuario.NombreUsuario,
                usuario.Correo,
                usuario.Rol,
                usuario.FechaCreacion
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
