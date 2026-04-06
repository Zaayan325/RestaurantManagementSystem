namespace RestaurantManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public ReportsViewModel DailyStats { get; set; } = new();
        
        public int AvailableTables { get; set; }
        public int OccupiedTables { get; set; }
        public int PendingOrders { get; set; }
        public int PreparingOrders { get; set; }
        public int AvailableItems { get; set; }
    }
}
