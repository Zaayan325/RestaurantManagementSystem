using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Services.Interface
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItem>> GetAllItemsAsync();
        Task<MenuItem?> GetItemByIdAsync(int id);
        Task CreateItemAsync(MenuItem item);
        Task UpdateItemAsync(MenuItem item);
        Task DeleteItemAsync(int id);
        Task ToggleAvailabilityAsync(int id);
        Task<IEnumerable<MenuItem>> GetAvailableItemsAsync();
    }
}
