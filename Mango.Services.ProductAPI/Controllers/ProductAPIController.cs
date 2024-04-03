using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Data;
using ProductAPI.Models.Dtos;
using ProductAPI.Utility;

namespace ProductAPI.Controllers;

[Route("api/product")]
[ApiController]
public class ProductAPIController : ControllerBase
{
    private readonly AppDbContext _dbCtx;
    private readonly IMapper _mapper;
    private readonly ResponseDto _response;

    public ProductAPIController(AppDbContext dbCtx, IMapper mapper)
    {
        _dbCtx = dbCtx;
        _mapper = mapper;
        _response = new ResponseDto();
    }

    [HttpGet]
    public ResponseDto Get()
    {
        try
        {
            var products = _dbCtx.Products.ToList();
            _response.Data = _mapper.Map<List<ProductDto>>(products);
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpGet("{id:int}")]
    public ResponseDto Get(int id)
    {
        try
        {
            var coupon = _dbCtx.Products.First(p => p.ProductId == id);
            _response.Data = _mapper.Map<ProductDto>(coupon);
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpPost]
    [Authorize(Roles = SD.RoleAdmin)]
    public ResponseDto Create(ProductDto productDto)
    {
        try
        {
            var product = _mapper.Map<Product>(productDto);
            _dbCtx.Products.Add(product);
            _dbCtx.SaveChanges();

            if (productDto.Image != null)
            {
                string filename = product.ProductId.ToString() + Path.GetExtension(productDto.Image.FileName);
                string filePath = @"wwwroot\ProductImages\" + filename;
                var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                using var fileStream = new FileStream(filePathDirectory, FileMode.Create);
                productDto.Image.CopyTo(fileStream);

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                product.ImageUrl = baseUrl + @"/ProductImages/" + filename;
                product.ImageLocalPath = filePath;
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }

            _dbCtx.Products.Update(product);
            _dbCtx.SaveChanges();
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpPut]
    [Authorize(Roles = SD.RoleAdmin)]
    public ResponseDto Update(ProductDto productDto)
    {
        try
        {
            var product = _mapper.Map<Product>(productDto);

            if (productDto.Image != null)
            {
                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo oldFile = new FileInfo(oldFilePath);

                    if (oldFile.Exists)
                    {
                        oldFile.Delete();
                    }
                }

                string filename = product.ProductId.ToString() + Path.GetExtension(productDto.Image.FileName);
                string filePath = @"wwwroot\ProductImages\" + filename;
                var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                using var fileStream = new FileStream(filePathDirectory, FileMode.Create);
                productDto.Image.CopyTo(fileStream);

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                product.ImageUrl = baseUrl + @"/ProductImages/" + filename;
                product.ImageLocalPath = filePath;
            }

            _dbCtx.Products.Update(product);
            _dbCtx.SaveChanges();
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = SD.RoleAdmin)]
    public ResponseDto Delete(int id)
    {
        try
        {
            var product = _dbCtx.Products.First(p => p.ProductId == id);
            if (!string.IsNullOrEmpty(product.ImageLocalPath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                FileInfo file = new FileInfo(filePath);

                if (file.Exists)
                {
                    file.Delete();
                }
            }

            _dbCtx.Products.Remove(product);
            _dbCtx.SaveChanges();
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }
}
