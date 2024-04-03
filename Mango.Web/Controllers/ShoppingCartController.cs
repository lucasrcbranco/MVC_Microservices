using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers;

public class ShoppingCartController : Controller
{
    private readonly IShoppingCartService _cartService;
    private readonly IOrderService _orderService;

    public ShoppingCartController(IShoppingCartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    [Authorize]
    public async Task<IActionResult> ShoppingCartIndex()
    {
        return View(await LoadShoppingCartDtoBasedOnLoggedInUser());
    }

    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        return View(await LoadShoppingCartDtoBasedOnLoggedInUser());
    }

    [HttpPost]
    [ActionName("Checkout")]
    public async Task<IActionResult> Checkout(ShoppingCartDto shoppingCartDto)
    {

        ShoppingCartDto cart = await LoadShoppingCartDtoBasedOnLoggedInUser();
        cart.ShoppingCartHeader.Email = shoppingCartDto.ShoppingCartHeader.Email;
        cart.ShoppingCartHeader.Name = shoppingCartDto.ShoppingCartHeader.Name;

        var response = await _orderService.CreateOrderAsync(cart);

        if (response != null && response.IsSuccess)
        {
            var domain = $"{Request.Scheme}://{Request.Host.Value}";
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Data));

            StripeRequestDto stripeRequestDto = new StripeRequestDto()
            {
                ApprovedUrl = $"{domain}/shoppingcart/confirmation?orderId={orderHeaderDto.OrderHeaderId}",
                CancelUrl = $"{domain}/checkout",
                OrderHeader = orderHeaderDto
            };

            var stripeResponse = await _orderService.CreateStripeSessionAsync(stripeRequestDto);
            StripeRequestDto sessionCreatedResponse = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Data));

            Response.Headers.Add("Location", sessionCreatedResponse.StripeSessionUrl);
            return new StatusCodeResult(303);
        }

        return View(cart);
    }

    [Authorize]
    public async Task<IActionResult> Confirmation(int orderId)
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDto? response = await _orderService.ValidateStripeSessionAsync(orderId);
        if (response != null & response.IsSuccess)
        {
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Data));

            if(orderHeaderDto.Status == SD.StatusApproved)
            {
                return View(orderId);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Remove(int cartDetailsId)
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
        if (response != null & response.IsSuccess)
        {
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(ShoppingCartIndex));
        }
        return RedirectToAction(nameof(ShoppingCartIndex));
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(ShoppingCartDto shoppingCartDto)
    {

        ResponseDto? response = await _cartService.ApplyCouponToCartAsync(shoppingCartDto);
        if (response != null & response.IsSuccess)
        {
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(ShoppingCartIndex));
        }
        return RedirectToAction(nameof(ShoppingCartIndex));
    }

    [HttpPost]
    public async Task<IActionResult> EmailCart(ShoppingCartDto ShoppingCartDto)
    {
        ShoppingCartDto cart = await LoadShoppingCartDtoBasedOnLoggedInUser();
        cart.ShoppingCartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
        ResponseDto? response = await _cartService.EmailCartAsync(cart);
        if (response != null & response.IsSuccess)
        {
            TempData["success"] = "Email will be processed and sent shortly.";
            return RedirectToAction(nameof(ShoppingCartIndex));
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(ShoppingCartDto ShoppingCartDto)
    {
        ShoppingCartDto.ShoppingCartHeader.CouponCode = "";
        ResponseDto? response = await _cartService.ApplyCouponToCartAsync(ShoppingCartDto);
        if (response != null & response.IsSuccess)
        {
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(ShoppingCartIndex));
        }
        return View();
    }


    private async Task<ShoppingCartDto> LoadShoppingCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);
        if (response != null & response.IsSuccess)
        {
            ShoppingCartDto ShoppingCartDto = JsonConvert.DeserializeObject<ShoppingCartDto>(Convert.ToString(response.Data));
            ShoppingCartDto.ShoppingCartHeader.Name = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Name)?.FirstOrDefault()?.Value;
            ShoppingCartDto.ShoppingCartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

            return ShoppingCartDto;
        }
        return new ShoppingCartDto();
    }
}