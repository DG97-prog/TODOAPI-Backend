using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TodoApp.API.DTOs;
using TodoApp.API.Interfaces;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var usuarioCreado = await _authService.RegistrarAsync(registerDto);
            if (usuarioCreado == null)
                return BadRequest("No se pudo registrar el usuario");

            return Ok(usuarioCreado);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto);
            if (token == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(new { token });
        }
    }
}
