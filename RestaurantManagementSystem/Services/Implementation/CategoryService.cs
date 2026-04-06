using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Repositories.Interface;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMenuRepository _menuRepo;

        public CategoryService(ICategoryRepository categoryRepo, IMenuRepository menuRepo)
        {
            _categoryRepo = categoryRepo;
            _menuRepo = menuRepo;
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync() => await _categoryRepo.GetAllAsync();

        public async Task<Category?> GetCategoryByIdAsync(int id) => await _categoryRepo.GetByIdAsync(id);

        public async Task CreateCategoryAsync(Category category)
        {
            // 1. Safety Guard: Ensure category isn't null
            if (category == null) throw new ArgumentNullException(nameof(category));

            // 2. Safety Guard: Force ID to 0 to ensure it's a "New" record
            category.Id = 0;

            try
            {
                await _categoryRepo.AddAsync(category);
            }
            catch (Exception ex)
            {
                // Put a breakpoint here to see the REAL database error!
                var msg = ex.Message;
                throw new Exception("Database Error: " + ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task UpdateCategoryAsync(Category category) => await _categoryRepo.UpdateAsync(category);

        public async Task DeleteCategoryAsync(int id)
        {
            var items = await _menuRepo.GetAllAsync();
            if (items.Any(i => i.CategoryId == id))
            {
                throw new Exception("Cannot delete category. Please delete or reassign all items in this category first.");
            }

            await _categoryRepo.DeleteAsync(id);
        }
    }   

}
