using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers;

public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public IActionResult OrderIndex()
    {
        return View();
    }

    public async Task<IActionResult> OrderDetail(int orderId)
    {
        OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
        string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

        var response = await _orderService.GetOrderById(orderId);
        if (response != null && response.IsSuccess)
        {
            orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Data));
        }

        if (!User.IsInRole(SD.RoleAdmin) && userId != orderHeaderDto.UserId)
        {
            return NotFound();
        }

        return View(orderHeaderDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(string status)
    {
        IEnumerable<OrderHeaderDto> orders;
        string userId = "";

        if (!User.IsInRole(SD.RoleAdmin))
        {
            userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;
        }

        ResponseDto? response = await _orderService.GetOrders(userId);
        if (response != null && response.IsSuccess)
        {
            orders = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(response.Data));

            switch (status)
            {
                case "approved":
                    orders = orders.Where(o => o.Status == SD.StatusApproved);
                    break;
                case "readyforpickup":
                    orders = orders.Where(o => o.Status == SD.StatusReadyForPickup);
                    break;
                case "cancelled":
                    orders = orders.Where(o => o.Status == SD.StatusCancelled);
                    break;
                default: 
                    break;
            }
        }
        else
        {
            orders = new List<OrderHeaderDto>();
        }

        return Json(new { data = orders });
    }

    [HttpPost("OrderReadyForPickup")]
    public async Task<IActionResult> OrderReadyForPickup(int orderId)
    {
        ResponseDto? response = await _orderService.UpdateStatus(orderId, SD.StatusReadyForPickup);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Status updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }

        return View();
    }

    [HttpPost("CompleteOrder")]
    public async Task<IActionResult> CompleteOrder(int orderId)
    {
        ResponseDto? response = await _orderService.UpdateStatus(orderId, SD.StatusCompleted);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Status updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }

        return View();
    }

    [HttpPost("CancelOrder")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        ResponseDto? response = await _orderService.UpdateStatus(orderId, SD.StatusCancelled);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Status updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }

        return View();
    }
}
