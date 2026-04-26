using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models.ViewModels
{
    public class RawMaterialStockUpdateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Raw Material")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Current Stock")]
        public decimal CurrentStock { get; set; }

        [Display(Name = "Unit")]
        public string Unit { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999, ErrorMessage = "Quantity must be greater than zero.")]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Display(Name = "Last Purchase Price")]
        [Range(0, 999999, ErrorMessage = "Price cannot be negative.")]
        public decimal? PurchasePrice { get; set; }
    }
}
