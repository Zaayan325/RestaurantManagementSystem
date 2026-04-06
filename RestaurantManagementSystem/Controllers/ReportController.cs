using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IOrderService _orderService;
        
        public ReportController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var reportData = await _orderService.GetReportDataAsync();
            return View(reportData);
        }
    }
}
