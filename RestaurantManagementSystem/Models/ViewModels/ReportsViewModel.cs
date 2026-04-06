namespace RestaurantManagementSystem.Models.ViewModels
{
    public class ReportsViewModel
    {
        public decimal TotalSalesToday { get; set; }
        public int TotalOrdersToday {  get; set; }
        public List<TopItemViewModel> TopSellingItems { get; set; } = new();
        public Dictionary<string, int> OrdersByServiceType { get; set; } = new();
    }
    public class TopItemViewModel
    {
        public string? ItemName { get; set; }
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }
}
