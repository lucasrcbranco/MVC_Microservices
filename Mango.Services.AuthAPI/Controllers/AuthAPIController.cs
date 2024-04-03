using AuthAPI.Integration;
using Mango.Services.AuthAPI.Models.Dtos;
using Mango.Services.AuthAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthAPIController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMessageBusService _messageBusService;
    private readonly ResponseDto _response;
    private readonly IConfiguration _configuration;

    public AuthAPIController(IAuthService authService, IMessageBusService messageBusService, IConfiguration configuration)
    {
        _authService = authService;
        _response = new();
        _messageBusService = messageBusService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
    {
        var errorMessage = await _authService.Register(registrationRequestDto);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            _response.IsSuccess = false;
            _response.Message = errorMessage;
            return BadRequest(_response);
        }

        try
        {
            await _messageBusService.PublishAsync(_configuration.GetValue<string>("TopicAndQueueNames:NewUserRegisteredQueue"), registrationRequestDto.Email);
        }
        catch
        {

        }

        _response.IsSuccess = true;
        return Ok(_response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var loginResponse = await _authService.Login(loginRequestDto);
        if (loginResponse.User == null)
        {
            _response.IsSuccess = false;
            _response.Message = "Login or password is incorrect";
            return BadRequest(_response);
        }

        _response.IsSuccess = true;
        _response.Data = loginResponse;
        return Ok(_response);
    }

    [HttpPost("assignRole")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequestDto assignRoleRequestDto)
    {
        var assignRoleResponse = await _authService.AssignRole(assignRoleRequestDto.Email, assignRoleRequestDto.RoleName.ToUpper());
        if (!assignRoleResponse)
        {
            _response.IsSuccess = false;
            _response.Message = "An error has ocurred while attempting to assign the role";
            return BadRequest(_response);
        }

        _response.IsSuccess = true;
        return Ok(_response);
    }
}
