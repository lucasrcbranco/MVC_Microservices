using Mango.Services.AuthAPI.Models.Dtos;

namespace Mango.Services.AuthAPI.Services.Interfaces;

public interface IAuthService
{
    Task<string> Register(RegistrationRequestDto registrationRequestDto);
    Task<LoginResponseDto> Login(LoginRequestDto registrationRequestDto);
    Task<bool> AssignRole(string email, string roleName);
}
