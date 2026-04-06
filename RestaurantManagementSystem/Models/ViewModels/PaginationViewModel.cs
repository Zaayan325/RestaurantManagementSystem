namespace RestaurantManagementSystem.Models.ViewModels
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Action { get; set; } = "Index";
        public string Controller { get; set; } = "";
    }
}
