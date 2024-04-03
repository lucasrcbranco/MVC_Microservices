using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly IShoppingCartService _shoppingCartService;

    public HomeController(ILogger<HomeController> logger, IProductService productService, IShoppingCartService shoppingCartService)
    {
        _logger = logger;
        _productService = productService;
        _shoppingCartService = shoppingCartService;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDto> products = new();
        ResponseDto? responseDto = await _productService.GetAllProductsAsync();
        if (responseDto != null && responseDto.IsSuccess)
        {
            products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responseDto.Data));
        }
        else
        {
            TempData["error"] = responseDto?.Message;
        }

        return View(products);
    }

    [Authorize]
    public async Task<IActionResult> ProductDetails(int productId)
    {
        ResponseDto? responseDto = await _productService.GetProductByIdAsync(productId);
        if (responseDto != null && responseDto.IsSuccess)
        {
            ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Data));
            return View(model);
        }
        else
        {
            TempData["error"] = responseDto?.Message;
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    [ActionName("ProductDetails")]
    public async Task<IActionResult> ProductDetails(ProductDto productDto)
    {
        ShoppingCartDto shoppingCartDto = new()
        {
            ShoppingCartHeader = new()
            {
                UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
            }
        };

        ShoppingCartDetailDto cartDetailDto = new()
        {
            Amount = productDto.Amount,
            ProductId = productDto.ProductId ?? 00
        };

        List<ShoppingCartDetailDto> cartDetailsDto = new() { cartDetailDto };
        shoppingCartDto.ShoppingCartDetails = cartDetailsDto;

        ResponseDto? responseDto = await _shoppingCartService.UpsertCartAsync(shoppingCartDto);
        if (responseDto != null && responseDto.IsSuccess)
        {
            TempData["success"] = "Item has been added to the Shopping Cart";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = responseDto?.Message;
        }

        return View(productDto);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
