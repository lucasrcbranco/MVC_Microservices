using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> ProductIndex()
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

    public async Task<IActionResult> ProductCreate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductDto model)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? responseDto = await _productService.CreateProductAsync(model);

            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
        }

        return View(model);
    }

    public async Task<IActionResult> ProductEdit(int productId)
    {
        if (ModelState.IsValid)
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
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductEdit(ProductDto model)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? responseDto = await _productService.UpdateProductAsync(model);

            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Product edited successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
        }

        return View(model);
    }

    public async Task<IActionResult> ProductDelete(int productId)
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

    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductDto productDto)
    {
        ResponseDto? responseDto = await _productService.DeleteProductAsync(productDto.ProductId!.Value);
        if (responseDto != null && responseDto.IsSuccess)
        {
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(ProductIndex));
        }
        else
        {
            TempData["error"] = responseDto?.Message;
        }

        return View(productDto);
    }
}
