using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenProvider _tokenProvider;

    public AuthController(IAuthService authService, ITokenProvider tokenProvider)
    {
        _authService = authService;
        _tokenProvider = tokenProvider;
    }

    [HttpGet]
    public IActionResult Register()
    {
        var roleList = new List<SelectListItem>() {
            new SelectListItem { Text=SD.RoleAdmin, Value= SD.RoleAdmin } ,
            new SelectListItem { Text=SD.RoleCustomer, Value= SD.RoleCustomer }
        };

        ViewBag.RoleList = roleList;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
    {
        ResponseDto registrationResult = await _authService.RegisterAsync(registrationRequestDto);

        if (registrationResult != null && registrationResult.IsSuccess)
        {
            if (string.IsNullOrEmpty(registrationRequestDto.Role))
            {
                registrationRequestDto.Role = SD.RoleCustomer;
            }

            var assignRoleResult = await _authService.AssingRoleAssync(new AssignRoleRequestDto() { Email = registrationRequestDto.Email, RoleName = registrationRequestDto.Role });
            if (assignRoleResult != null && assignRoleResult.IsSuccess)
            {
                TempData["success"] = "Registration Successful";
                return RedirectToAction(nameof(Login));
            }
        }
        else
        {
            TempData["error"] = registrationResult.Message;
        }

        var roleList = new List<SelectListItem>() {
            new SelectListItem { Text=SD.RoleAdmin, Value= SD.RoleAdmin } ,
            new SelectListItem { Text=SD.RoleCustomer, Value= SD.RoleCustomer }
        };

        ViewBag.RoleList = roleList;

        return View(registrationRequestDto);
    }

    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDto loginRequestDto = new();
        return View(loginRequestDto);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
    {
        ResponseDto loginResult = await _authService.LoginAsync(loginRequestDto);

        if (loginResult != null && loginResult.IsSuccess)
        {
            LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(loginResult.Data));
            _tokenProvider.SetToken(loginResponseDto.Token);
            await SignInUserAsync(loginResponseDto);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            TempData["error"] = loginResult.Message;
            return View(loginRequestDto);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        _tokenProvider.ClearToken();
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUserAsync(LoginResponseDto loginResponseDto)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(loginResponseDto.Token);
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, token.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Sub).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, token.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Email).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, token.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Name).Value));

        identity.AddClaim(new Claim(ClaimTypes.Name, token.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Email).Value));
        identity.AddClaim(new Claim(ClaimTypes.Role, token.Claims.FirstOrDefault(t => t.Type == "role").Value));


        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
