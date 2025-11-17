using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TodoApp.API.DTOs;
using TodoApp.API.Interfaces;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

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
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto);
            if (token == null)
                return Unauthorized(new { mensaje = "Nombre de usuario o contraseña incorrectos." });

            return Ok(new { token });
        }
    }
}
