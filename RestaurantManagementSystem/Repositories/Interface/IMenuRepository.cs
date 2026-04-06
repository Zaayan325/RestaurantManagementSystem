using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Repositories.Interface
{
    public interface IMenuRepository : IGenericRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetMenuItemsWithCategoriesAsync();
    }
}
