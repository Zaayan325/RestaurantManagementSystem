using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Models.ViewModels;
using RestaurantManagementSystem.Repositories.Interface;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITableService _tableService;
        private readonly IGenericRepository<MenuItem> _menuRepo;
        private readonly ApplicationDbContext _context;

        public OrderService(IOrderRepository orderRepository, ITableService tableService, IGenericRepository<MenuItem> menuRepo, ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _tableService = tableService;
            _menuRepo = menuRepo;
            _context = context;
        }

        public async Task CreateOrderAsync(Order order)
        {
            decimal Total = 0;
            foreach (var item in order.OrderItems)
            {
                // Look up the current price from the Menu
                var menuItem = await _menuRepo.GetByIdAsync(item.MenuItemId);
                if (menuItem != null)
                {
                    item.UnitPrice = menuItem.Price;
                    Total += item.UnitPrice * item.Quantity;
                }
            }
            order.TotalAmount = Total;
            await _orderRepository.AddAsync(order);
            // If Dine-In, mark the table as Occupied
            if (order.ServiceType == ServiceType.DineIn && order.TableId.HasValue)
            {
                await _tableService.UpdateTableStatusAsync(order.TableId.Value, TableStatus.Occupied);
            }
        }

        public async Task<(bool Success, string Message)> UpdateStatusAsync(int orderId, OrderStatus status, string userRole)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return (false, "Order not found"); ;

            // --- ROLE VALIDATION LOGIC ---
            // Rule: Chef can only mark as Preparing or Ready
            if (userRole == "Chef" && status != OrderStatus.Preparing && status != OrderStatus.Ready)
            {
                throw new UnauthorizedAccessException("Chefs can only update orders to Preparing or Ready.");
            }

            // Rule: Waiter cannot touch the kitchen cooking status
            if (userRole == "Waiter" && (status == OrderStatus.Preparing || status == OrderStatus.Ready))
            {
                throw new UnauthorizedAccessException("Waiters cannot manage kitchen status.");
            }

            // --- STATUS UPDATE LOGIC ---
            order.Status = status;
            await _orderRepository.UpdateAsync(order);

            // Notification Logic
            string msg = status switch
            {
                OrderStatus.Confirmed => "New Order Confirmed! Chef, please check the kitchen display.",
                OrderStatus.Preparing => "Order is now being Prepared. Waiter, stay alert!",
                OrderStatus.Ready => "Order is READY! Waiter, please serve Table " + (order.Table?.TableName ?? "N/A"),
                OrderStatus.Completed => "Order Paid and Table Cleared.",
                _ => "Order status updated."
            };

            // Table Management
            if (order.TableId.HasValue)
            {
                if (status == OrderStatus.Completed || status == OrderStatus.Cancelled)
                {
                    await _tableService.UpdateTableStatusAsync(order.TableId.Value, TableStatus.Available);
                }
                else if (status == OrderStatus.Confirmed)
                {
                    await _tableService.UpdateTableStatusAsync(order.TableId.Value, TableStatus.Occupied);
                }
            }
            return (true, msg);
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _orderRepository.GetOrdersWithDetailAsync();
        public async Task<Order?> GetOrderByIdAsync(int id) => await _orderRepository.GetOrderByIdWithDetailsAsync(id);

        public async Task<ReportsViewModel> GetReportDataAsync()
        {
            var today = DateTime.Today;

            // Get Completed Orders for today
            var ordersToday = await _orderRepository.GetOrdersWithDetailAsync();
            var filteredOrders = ordersToday
                .Where(o => o.OrderDate.Date == today && o.Status == OrderStatus.Completed)
                .ToList();

            //Build the ViewModel
            var dashboard = new ReportsViewModel
            {
                TotalSalesToday = filteredOrders.Sum(o => o.TotalAmount),
                TotalOrdersToday = filteredOrders.Count,

                //Logic for the PieChart
                OrdersByServiceType = filteredOrders
                    .GroupBy(o => o.ServiceType)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count()),

                //Logic for Top Selling Items
                TopSellingItems = filteredOrders
                    .SelectMany(o => o.OrderItems)
                    .GroupBy(oi => oi.MenuItem.Name)
                    .Select(group => new TopItemViewModel
                    {
                        ItemName = group.Key,
                        Count = group.Sum(x => x.Quantity),
                        Revenue = group.Sum(x => x.Quantity * x.UnitPrice)
                    })
                    .OrderByDescending(x => x.Revenue)
                    .Take(5)
                    .ToList()
            };
            return dashboard;
        }
        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var stats = await GetReportDataAsync();

            return new DashboardViewModel
            {
                DailyStats = stats,
                AvailableTables = await _context.Tables.CountAsync(t => t.Status == TableStatus.Available),
                OccupiedTables = await _context.Tables.CountAsync(t => t.Status == TableStatus.Occupied),
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending),
                PreparingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Preparing),
                AvailableItems = await _context.MenuItems.CountAsync(m => m.IsAvailable)
            };
        }

        //Pagination
        public async Task<(IEnumerable<Order> Orders, int TotalPages)> GetPagedOrdersAsync(int page, int pageSize)
        {
            // 1. Get the total count of orders to calculate total pages
            var totalCount = await _context.Orders.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // 2. Fetch the specific page with sorting
            var orders = await _context.Orders
                .Include(o => o.Table)        // Ensure Table info is loaded for the View
                .OrderByDescending(o => o.OrderDate) // NEWEST RECORDS AT THE TOP
                .Skip((page - 1) * pageSize)   // Skip previous pages
                .Take(pageSize)                // Only take the current page's amount
                .ToListAsync();

            return (orders, totalPages);
        }
    }
}
