using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Repositories.Interface;

namespace RestaurantManagementSystem.Repositories.Implementation
{
    public class TableRepository : GenericRepository<Table>, ITableRepository
    {
        public TableRepository(ApplicationDbContext context) : base(context) { }
    }
}
