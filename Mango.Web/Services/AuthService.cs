using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;

namespace Mango.Web.Services;

public class AuthService : IAuthService
{
    private readonly IBaseService _baseService;

    public AuthService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> AssingRoleAssync(AssignRoleRequestDto assignRoleRequestDto)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = assignRoleRequestDto,
            URL = SD.AuthAPIBase + "/api/auth/assignRole"
        }, requiresAuthentication: false);
    }

    public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = loginRequestDto,
            URL = SD.AuthAPIBase + "/api/auth/login"
        }, requiresAuthentication: false);
    }

    public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = registrationRequestDto,
            URL = SD.AuthAPIBase + "/api/auth/register"
        }, requiresAuthentication: false);
    }
}
