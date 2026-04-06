using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        readonly IOrderService _orderService;
        readonly ITableService _tableService;
        readonly IMenuService _menuService;

        public OrderController(IOrderService orderService, ITableService tableService, IMenuService menuService)
        {
            _orderService = orderService;
            _tableService = tableService;
            _menuService = menuService;
        }

        //View
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 8;
            var (orders, totalPages) = await _orderService.GetPagedOrdersAsync(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(orders);
        }

        //Details
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        //Create
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> Create()
        {
            //DropDowns
            var tables = await _tableService.GetAllTablesAsync();
            var menuItems = await _menuService.GetAvailableItemsAsync();

            ViewBag.Tables = new SelectList(tables, "Id", "TableName");
            ViewBag.MenuItems = menuItems;
            return View(new Order());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            // We remove validation for TotalAmount because the Service calculates it
            ModelState.Remove("TotalAmount");
            ModelState.Remove("Status");

            if (ModelState.IsValid)
            {
                await _orderService.CreateOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }
            // If we fail, reload the dropdowns
            ViewBag.Tables = new SelectList(await _tableService.GetAllTablesAsync(), "Id", "TableName");
            return View(order);
        }

        //Update Status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            try
            {
                // 1. Identify the role of the logged-in user
                string userRole = User.IsInRole("Admin") ? "Admin" :
                                 User.IsInRole("Chef") ? "Chef" : "Waiter";

                // 2. Call the service and capture the (Success, Message) tuple
                var result = await _orderService.UpdateStatusAsync(id, status, userRole);

                if (result.Success)
                {
                    // Success Notification for the Toast
                    TempData["Notification"] = result.Message;
                }
                else
                {
                    // Logic failure (e.g., Order not found)
                    TempData["Error"] = result.Message;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Security/Role violation caught from the Service
                TempData["Error"] = ex.Message;
            }
            catch (Exception)
            {
                // General fallback for unexpected database errors
                TempData["Error"] = "An unexpected error occurred while updating the order.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
