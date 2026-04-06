using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Repositories.Interface
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersWithDetailAsync();
        Task<Order?> GetOrderByIdWithDetailsAsync(int id);
    }
}
