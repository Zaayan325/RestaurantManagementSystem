using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Services.Interface
{
    public interface IRawMaterialService
    {
        Task<IEnumerable<RawMaterial>> GetAllAsync();
        Task<IEnumerable<RawMaterial>> GetLowStockItemsAsync();
        Task<RawMaterial?> GetByIdAsync(int id);
        Task CreateAsync(RawMaterial material);
        Task UpdateAsync(RawMaterial material);
        Task DeleteAsync(int id);
        Task AddStockAsync(int id, decimal quantity, decimal? purchasePrice);
        Task ConsumeStockAsync(int id, decimal quantity);
    }
}
