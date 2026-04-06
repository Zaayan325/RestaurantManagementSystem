using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Repositories.Implementation
{
    public class MenuRepository : GenericRepository<MenuItem>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MenuItem>> GetMenuItemsWithCategoriesAsync()
        {
            return await _context.MenuItems.Include(m => m.Category).ToListAsync();
        }

    }
}
