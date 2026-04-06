using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Repositories.Interface;

namespace RestaurantManagementSystem.Repositories.Implementation
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }
    }
}
