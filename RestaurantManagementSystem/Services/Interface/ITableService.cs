using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Services.Interface
{
    public interface ITableService
    {
        Task<IEnumerable<Table>> GetAllTablesAsync();
        Task<Table?> GetTableByIdAsync(int id);
        Task CreateTableAsync(Table table);
        Task UpdateTableAsync(Table table);
        Task DeleteTableAsync(int id);
        Task UpdateTableStatusAsync(int tableId,  TableStatus status);
    }
}
