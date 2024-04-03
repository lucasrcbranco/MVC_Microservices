using AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dtos;
using Mango.Services.AuthAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        AppDbContext appDbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<bool> AssignRole(string email, string roleName)
    {
        try
        {
            ApplicationUser user = _appDbContext.ApplicationUsers.First(u => u.NormalizedEmail == email.ToUpper());

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        try
        {
            ApplicationUser user = _appDbContext.ApplicationUsers.First(u => u.NormalizedEmail == loginRequestDto.Email.ToUpper());
            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!isPasswordValid)
            {
                return new();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            return new()
            {
                User = new UserDto() { ID = user.Id, Email = user.Email, Name = user.Name },
                Token = token
            };
        }
        catch
        {
            return new();
        }
    }

    public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
    {
        ApplicationUser user = new()
        {
            UserName = registrationRequestDto.Email,
            Email = registrationRequestDto.Email,
            NormalizedEmail = registrationRequestDto.Email.ToUpper(),
            Name = registrationRequestDto.Name
        };

        try
        {
            var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
            if (result.Succeeded)
            {
                var userToReturn = _appDbContext.ApplicationUsers.First(u => u.NormalizedEmail == registrationRequestDto.Email.ToUpper());
                return string.Empty;
            }
            else
            {
                return result.Errors.FirstOrDefault()?.Description;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
