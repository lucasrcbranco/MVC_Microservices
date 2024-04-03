namespace Mango.Services.AuthAPI.Models.Dtos;

public class RegistrationRequestDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
