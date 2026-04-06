using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Models.ViewModels;

namespace RestaurantManagementSystem.Services.Interface
{
    public interface IOrderService
    {
        Task CreateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<(bool Success, string Message)> UpdateStatusAsync(int  orderId, OrderStatus status, string userRole);

        Task<ReportsViewModel> GetReportDataAsync();
        Task<DashboardViewModel> GetDashboardDataAsync();

        Task<(IEnumerable<Order> Orders, int TotalPages)> GetPagedOrdersAsync(int page, int pageSize);

    }
}
