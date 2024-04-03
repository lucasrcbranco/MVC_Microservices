using Mango.Services.EmailAPI.Models.Dtos;

namespace Mango.Services.EmailAPI.Services;

public interface IEmailService
{
    Task LogEmailAsync(ShoppingCartDto shoppingCartDto);
    Task LogNewUserRegisteredAsync(string email);
}
