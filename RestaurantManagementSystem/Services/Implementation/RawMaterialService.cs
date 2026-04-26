using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Repositories.Interface;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Services.Implementation
{
    public class RawMaterialService : IRawMaterialService
    {
        private readonly IGenericRepository<RawMaterial> _rawMaterialRepo;

        public RawMaterialService(IGenericRepository<RawMaterial> rawMaterialRepo)
        {
            _rawMaterialRepo = rawMaterialRepo;
        }

        public async Task<IEnumerable<RawMaterial>> GetAllAsync()
        {
            var materials = await _rawMaterialRepo.GetAllAsync();
            return materials.OrderBy(m => m.Name);
        }

        public async Task<IEnumerable<RawMaterial>> GetLowStockItemsAsync()
        {
            var materials = await _rawMaterialRepo.GetAllAsync();
            return materials
                .Where(m => m.IsActive && m.CurrentStock <= m.ReorderLevel)
                .OrderBy(m => m.CurrentStock);
        }

        public async Task<RawMaterial?> GetByIdAsync(int id)
        {
            return await _rawMaterialRepo.GetByIdAsync(id);
        }

        public async Task CreateAsync(RawMaterial material)
        {
            if (material == null) throw new ArgumentNullException(nameof(material));
            material.Id = 0;
            await _rawMaterialRepo.AddAsync(material);
        }

        public async Task UpdateAsync(RawMaterial material)
        {
            if (material == null) throw new ArgumentNullException(nameof(material));
            await _rawMaterialRepo.UpdateAsync(material);
        }

        public async Task DeleteAsync(int id)
        {
            await _rawMaterialRepo.DeleteAsync(id);
        }

        public async Task AddStockAsync(int id, decimal quantity, decimal? purchasePrice)
        {
            if (quantity <= 0) throw new Exception("Quantity must be greater than zero.");

            var material = await _rawMaterialRepo.GetByIdAsync(id);
            if (material == null) throw new Exception("Raw material not found.");

            material.CurrentStock += quantity;
            material.LastPurchasedOn = DateTime.UtcNow;

            if (purchasePrice.HasValue && purchasePrice.Value > 0)
            {
                material.LastPurchasePrice = purchasePrice.Value;
            }

            await _rawMaterialRepo.UpdateAsync(material);
        }

        public async Task ConsumeStockAsync(int id, decimal quantity)
        {
            if (quantity <= 0) throw new Exception("Quantity must be greater than zero.");

            var material = await _rawMaterialRepo.GetByIdAsync(id);
            if (material == null) throw new Exception("Raw material not found.");
            if (material.CurrentStock < quantity) throw new Exception("Not enough stock available.");

            material.CurrentStock -= quantity;
            await _rawMaterialRepo.UpdateAsync(material);
        }
    }
}
