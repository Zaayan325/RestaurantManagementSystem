using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Repositories.Interface;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Services.Implementation
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepo;

        public MenuService(IMenuRepository menuRepo)
        {
            _menuRepo = menuRepo;
        }

        public async Task<IEnumerable<MenuItem>> GetAllItemsAsync()
        {
            return await _menuRepo.GetMenuItemsWithCategoriesAsync();
        }

        public async Task<MenuItem?> GetItemByIdAsync(int id) => await _menuRepo.GetByIdAsync(id);

        public async Task CreateItemAsync(MenuItem item)
        {
            // 1. Safety Guard: Ensure item isn't null
            if (item == null) throw new ArgumentNullException(nameof(item));

            // 2. Business Rule: Price cannot be zero or negative
            if (item.Price <= 0) throw new Exception("Price must be greater than zero.");

            // 3. Safety Guard: Force ID to 0 to ensure EF treats this as a "New" record
            // This prevents "Id already tracked" errors if the form sends a weird value.
            item.Id = 0;

            try
            {
                await _menuRepo.AddAsync(item);
            }
            catch (Exception ex)
            {
                // LOGIC: We extract the 'InnerException' because that's where 
                // SQL Server hides the real reason for the failure.
                var detailedError = ex.InnerException?.Message ?? ex.Message;

                // Put a breakpoint on the line below to see 'detailedError' in your debugger
                throw new Exception($"Database Error while creating Menu Item: {detailedError}");
            }
        }

        public async Task UpdateItemAsync(MenuItem item) => await _menuRepo.UpdateAsync(item);

        public async Task DeleteItemAsync(int id) => await _menuRepo.DeleteAsync(id);

        public async Task ToggleAvailabilityAsync(int id)
        {
            var item = await _menuRepo.GetByIdAsync(id);
            if (item != null)
            {
                item.IsAvailable = !item.IsAvailable;
                await _menuRepo.UpdateAsync(item);
            }
        }
        public async Task<IEnumerable<MenuItem>> GetAvailableItemsAsync()
        {
            var allItems = await _menuRepo.GetMenuItemsWithCategoriesAsync();
            return allItems.Where(m => m.IsAvailable).ToList();
        }
    }
}
