namespace Mango.Services.AuthAPI.Models.Dtos;

public class AssignRoleRequestDto
{
    public string Email { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}