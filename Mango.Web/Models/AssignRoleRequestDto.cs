namespace Mango.Web.Models;

public class AssignRoleRequestDto
{
    public string Email { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}