using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Repositories.Interface;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Services.Implementation
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepo;

        public TableService(ITableRepository tableRepo)
        {
            _tableRepo = tableRepo;
        }

        public async Task<IEnumerable<Table>> GetAllTablesAsync() => await _tableRepo.GetAllAsync();

        public async Task<Table?> GetTableByIdAsync(int id) => await _tableRepo.GetByIdAsync(id);

        public async Task CreateTableAsync(Table table)
        {
            // Business Rule: Table must have a name and at least 1 seat
            if (string.IsNullOrEmpty(table.TableName)) throw new Exception("Table name is required.");
            if (table.Capacity <= 0) throw new Exception("Capacity must be atleast 1.");
            await _tableRepo.AddAsync(table);
        }

        public async Task UpdateTableAsync(Table table) => await _tableRepo.UpdateAsync(table);
        public async Task DeleteTableAsync(int id)
        {
            var table = await _tableRepo.GetByIdAsync(id);
            if (table != null & table.Status != TableStatus.Available)
            {
                throw new Exception("Cannot delete a table that is currently occupied");
            }
            await _tableRepo.DeleteAsync(id);
        }
        public async Task UpdateTableStatusAsync(int tableId, TableStatus status)
        {
            var table = await _tableRepo.GetByIdAsync(tableId);
            if (table != null)
            {
                table.Status = status;
                await _tableRepo.UpdateAsync(table);
            }
        }
    }
}
