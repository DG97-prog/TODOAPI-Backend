using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoApp.API.Data;
using TodoApp.API.Models;
using TodoApp.API.DTOs;
using TodoApp.API.Interfaces;
using ClosedXML.Excel;

namespace TodoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public TareasController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

            if (tarea.UsuarioId != userId)
                return Forbid(); 

            return Ok(tarea);
        }

        // POST: api/tareas
        [HttpPost]
        public async Task<ActionResult<Tarea>> CreateTask(CreateTareaDto dto)
        {
            var userId = GetUserId();
            var role = GetUserRole();

            int usuarioAsignadoId = (role == "Supervisor" && dto.UsuarioId.HasValue)
                ? dto.UsuarioId.Value
                : userId;

            var usuarioAsignado = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioAsignadoId);

            if (usuarioAsignado == null)
                return BadRequest("El usuario asignado no existe.");

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
                UsuarioId = usuarioAsignadoId,
                FechaCreacion = DateTime.UtcNow,
                FechaVencimiento = dto.FechaVencimiento
            };

            try
            {
                _context.Tareas.Add(tarea);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Aquí puedes loguear en consola o en logs
                Console.WriteLine("DB ERROR: " + (ex.InnerException?.Message ?? ex.Message));
                throw;
            }


            try
            {
                var creador = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var esMismaPersona = userId == usuarioAsignadoId;

                var subject = esMismaPersona
                    ? "Nueva tarea asignada a ti"
                    : $"Te asignaron una nueva tarea (por {creador?.NombreUsuario})";

                var body = esMismaPersona
                    ? $"Hola {usuarioAsignado.NombreUsuario},\n\n" +
                      $"Se ha creado una nueva tarea para ti:\n\n" +
                      $"- Título: {tarea.Titulo}\n" +
                      $"- Descripción: {tarea.Descripcion}\n" +
                      $"- Fecha vencimiento: {tarea.FechaVencimiento:yyyy-MM-dd HH:mm}\n\n" +
                      "Ingresa a la aplicación para más detalles."
                    : $"Hola {usuarioAsignado.NombreUsuario},\n\n" +
                      $"El usuario {creador?.NombreUsuario} te ha asignado una nueva tarea:\n\n" +
                      $"- Título: {tarea.Titulo}\n" +
                      $"- Descripción: {tarea.Descripcion}\n" +
                      $"- Fecha vencimiento: {tarea.FechaVencimiento:yyyy-MM-dd HH:mm}\n\n" +
                      "Ingresa a la aplicación para más detalles.";

                await _emailService.SendEmailAsync(
                    usuarioAsignado.Correo,
                    subject,
                    body
                );
            }
            catch (Exception ex)
            {
                // No rompemos la creación de tarea si falla el correo, solo log
                Console.WriteLine($"Error enviando correo: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetTarea), new { id = tarea.Id }, tarea);
        }



        // PUT: api/tareas/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Tarea>> ActualizarTarea(int id, UpdateTareaDto dto)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            var userId = GetUserId();

            if (tarea.UsuarioId != userId)
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

            return Ok(tarea);
        }


        // DELETE: api/tareas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            var userId = GetUserId();

            if (tarea.UsuarioId != userId)
                return Forbid();

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("report")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DownloadReport()
        {
            var tareas = await _context.Tareas
                .Include(t => t.Categoria)
                .Include(t => t.Estado)
                .Include(t => t.Usuario)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reporte de Tareas");

            // Encabezados
            ws.Cell(1, 1).Value = "Título";
            ws.Cell(1, 2).Value = "Descripción";
            ws.Cell(1, 3).Value = "Categoría";
            ws.Cell(1, 4).Value = "Estado";
            ws.Cell(1, 5).Value = "Usuario Asignado";
            ws.Cell(1, 6).Value = "Fecha Creación";
            ws.Cell(1, 7).Value = "Fecha Vencimiento";

            int row = 2;

            foreach (var t in tareas)
            {
                ws.Cell(row, 1).Value = t.Titulo;
                ws.Cell(row, 2).Value = t.Descripcion;
                ws.Cell(row, 3).Value = t.Categoria?.Nombre;
                ws.Cell(row, 4).Value = t.Estado?.NombreEstado;
                ws.Cell(row, 5).Value = t.Usuario?.NombreUsuario;
                ws.Cell(row, 6).Value = t.FechaCreacion.ToString("yyyy-MM-dd HH:mm");
                ws.Cell(row, 7).Value = t.FechaVencimiento?.ToString("yyyy-MM-dd HH:mm"); 

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"reporte_tareas_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }


        // Métodos auxiliares
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
