using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoApp.API.Data;
using TodoApp.API.Models;
using TodoApp.API.DTOs;

namespace TodoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Descomenta si quieres que solo usuarios autenticados accedan
    
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TareasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("prueba")]
        [AllowAnonymous]
        public IActionResult Prueba()
        {
            return Ok("¡API funciona sin autorización!");
        }

        // GET: api/tareas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas()
        {
            var userId = GetUserId();

            var tareas = await _context.Tareas
                .Include(t => t.Categoria)
                .Include(t => t.Estado)
                .Where(t => t.UsuarioId == userId)
                .ToListAsync();

            return Ok(tareas);
        }

        // GET: api/tareas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetTarea(int id)
        {
            var tarea = await _context.Tareas
                .Include(t => t.Categoria)
                .Include(t => t.Estado)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tarea == null) return NotFound();

            var userId = GetUserId();
            var userRole = GetUserRole();

            if (tarea.UsuarioId != userId && userRole != "Admin")
                return Forbid();

            return Ok(tarea);
        }

        // POST: api/tareas
        [HttpPost]
        public async Task<ActionResult<Tarea>> CrearTarea(CreateTareaDto dto)
        {
            var userId = GetUserId();

            // Verificar que el usuario existe en la DB (evitar FK violation)
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == userId);
            if (!usuarioExiste)
                return BadRequest("El usuario autenticado no existe en la base de datos.");

            // Verificar que Categoria y Estado existen para evitar error FK
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            if (!categoriaExiste)
                return BadRequest("La categoría especificada no existe.");

            var estadoExiste = await _context.Estados.AnyAsync(e => e.Id == dto.EstadoId);
            if (!estadoExiste)
                return BadRequest("El estado especificado no existe.");

            var tarea = new Tarea
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                CategoriaId = dto.CategoriaId,
                EstadoId = dto.EstadoId,
                UsuarioId = userId,
                FechaCreacion = DateTime.UtcNow,
                FechaVencimiento = dto.FechaVencimiento
            };

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTarea), new { id = tarea.Id }, tarea);
        }

        // PUT: api/tareas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarTarea(int id, UpdateTareaDto dto)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            var userId = GetUserId();
            var userRole = GetUserRole();

            if (tarea.UsuarioId != userId && userRole != "Admin")
                return Forbid();

            // Validar existencia de Categoria y Estado nuevos
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            if (!categoriaExiste)
                return BadRequest("La categoría especificada no existe.");

            var estadoExiste = await _context.Estados.AnyAsync(e => e.Id == dto.EstadoId);
            if (!estadoExiste)
                return BadRequest("El estado especificado no existe.");

            tarea.Titulo = dto.Titulo;
            tarea.Descripcion = dto.Descripcion;
            tarea.CategoriaId = dto.CategoriaId;
            tarea.EstadoId = dto.EstadoId;
            tarea.FechaVencimiento = dto.FechaVencimiento;

            _context.Tareas.Update(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/tareas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            var userId = GetUserId();
            var userRole = GetUserRole();

            if (tarea.UsuarioId != userId && userRole != "Admin")
                return Forbid();

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Métodos auxiliares para obtener info del usuario logueado
        private int GetUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdValue) || !int.TryParse(userIdValue, out int userId))
                throw new UnauthorizedAccessException("Usuario no autenticado o token inválido.");
            return userId;
        }

        private string GetUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role) ?? "";
        }
    }
}
