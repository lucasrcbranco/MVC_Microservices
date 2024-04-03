using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CouponController : Controller
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<IActionResult> CouponIndex()
    {
        List<CouponDto> coupons = new();
        ResponseDto? responseDto = await _couponService.GetAllCouponsAsync();
        if (responseDto != null && responseDto.IsSuccess)
        {
            coupons = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(responseDto.Data));
        }
        else
        {
            TempData["error"] = responseDto?.Message;
        }

        return View(coupons);
    }

    public async Task<IActionResult> CouponCreate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CouponCreate(CouponDto model)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? responseDto = await _couponService.CreateCouponAsync(model);

            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Coupon created successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
        }

        return View(model);
    }

    public async Task<IActionResult> CouponDelete(int couponId)
    {
        ResponseDto? responseDto = await _couponService.GetCouponByIdAsync(couponId);
        if (responseDto != null && responseDto.IsSuccess)
        {
            CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseDto.Data));
            return View(model);
        }
        else
        {
            TempData["error"] = responseDto?.Message;
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CouponDelete(CouponDto couponDto)
    {
        ResponseDto? responseDto = await _couponService.DeleteCouponAsync(couponDto.CouponId);
        if (responseDto != null && responseDto.IsSuccess)
        {
            TempData["success"] = "Coupon deleted successfully";
            return RedirectToAction(nameof(CouponIndex));
        }
        else
        {
            TempData["error"] = responseDto?.Message;
        }

        return View(couponDto);
    }
}
