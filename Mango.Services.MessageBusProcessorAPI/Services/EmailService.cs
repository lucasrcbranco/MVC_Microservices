using EmailAPI.Data;
using Mango.Services.EmailAPI.Messages;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services;

public class EmailService : IEmailService
{
    private const string AdminEmail = "lucas@teste.com";
    private DbContextOptions<AppDbContext> _appDbCtxOptions;

    public EmailService(DbContextOptions<AppDbContext> appDbCtxOptions)
    {
        _appDbCtxOptions = appDbCtxOptions;
    }

    public async Task LogNewUserRegisteredAsync(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email must not be null or empty");

            EmailLogger emailLogger = new()
            {
                Email = AdminEmail,
                Message = GenerateBodyFromEmailForNewUserRegistered(email)
            };

            await using var _dbCtx = new AppDbContext(_appDbCtxOptions);
            await _dbCtx.EmailLoggers.AddAsync(emailLogger);
            await _dbCtx.SaveChangesAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task LogMessageAsync(string message)
    {
        try
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message must not be null or empty");

            EmailLogger logger = new()
            {
                Email = AdminEmail,
                Message = message
            };

            await using var _dbCtx = new AppDbContext(_appDbCtxOptions);
            await _dbCtx.EmailLoggers.AddAsync(logger);
            await _dbCtx.SaveChangesAsync();
        }
        catch
        {
            throw;
        }
    }


    private string GenerateBodyFromEmailForNewUserRegistered(string email)
    {
        return $"A new user has been registered to the email {email}";
    }

    public async Task LogEmailAsync(ShoppingCartDto shoppingCartDto)
    {
        try
        {
            if (shoppingCartDto is null)
                throw new ArgumentException("ShoppingCartDto must not be null");

            if (shoppingCartDto.ShoppingCartHeader is null)
                throw new ArgumentException("ShoppingCartDto.ShoppingCartHeader must not be null");

            if (string.IsNullOrEmpty(shoppingCartDto.ShoppingCartHeader.Email))
                throw new ArgumentException("ShoppingCartDto.ShoppingCartHeader.Email must not be null or empty");

            EmailLogger emailLogger = new()
            {
                Email = shoppingCartDto.ShoppingCartHeader.Email,
                Message = GenerateEmailBodyFromShoppingCart(shoppingCartDto)
            };

            await using var _dbCtx = new AppDbContext(_appDbCtxOptions);
            await _dbCtx.EmailLoggers.AddAsync(emailLogger);
            await _dbCtx.SaveChangesAsync();
        }
        catch
        {
            throw;
        }
    }

    private string GenerateEmailBodyFromShoppingCart(ShoppingCartDto shoppingCartDto)
    {
        StringBuilder emailBody = new();
        emailBody.AppendLine("<br/> Cart Email Requested");
        emailBody.AppendLine($"<br/> Original Price of the Cart: {shoppingCartDto.ShoppingCartHeader.Price.ToString("C")}");
        emailBody.AppendLine($"<br/> Discounted Granted: {shoppingCartDto.ShoppingCartHeader.Discount.ToString("C")}");
        emailBody.AppendLine($"<br/> Total Value of the Cart: {shoppingCartDto.ShoppingCartHeader.CartTotal.ToString("C")}");
        emailBody.AppendLine("<br/>");

        if (shoppingCartDto.ShoppingCartDetails != null && shoppingCartDto.ShoppingCartDetails.Any())
        {
            emailBody.Append("<ul>");

            foreach (var item in shoppingCartDto.ShoppingCartDetails)
            {
                if (item.Product is null) continue;

                emailBody.Append("<li>");
                emailBody.Append($"{item.Product.Name} x {item.Amount} Un x {item.Product.Price.ToString("C")}");
                emailBody.Append("</li>");
            }

            emailBody.Append("</ul>");
        }

        return emailBody.ToString();
    }

    public async Task LogOrderPlaced(RewardMessage rewardMessage)
    {
        string message = $"New Order Placed. <br/> Order ID: ${rewardMessage.OrderId}";
        await LogMessageAsync(message);
    }
}
