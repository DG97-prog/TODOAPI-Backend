using TodoApp.API.DTOs;
using TodoApp.API.Models;
using System.Threading.Tasks;

namespace TodoApp.API.Interfaces
{
    public interface IAuthService
    {
        Task<Usuario> RegistrarAsync(RegisterDto registerDto);
        Task<string?> LoginAsync(LoginDto loginDto);
    }
}
