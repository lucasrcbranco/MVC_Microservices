using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthAPI.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string Name { get; set; } = null!;
}
