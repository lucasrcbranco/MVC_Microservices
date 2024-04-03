﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models;

public class LoginRequestDto
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}