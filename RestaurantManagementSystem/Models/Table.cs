using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RestaurantManagementSystem.Models
{
    public enum TableStatus
    {
        Available, Occupied, Reserved, Cleaning
    }

    public class Table
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Table Number is Required")]
        [Display(Name ="Table Number")]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [Range(1, 20, ErrorMessage ="Capacity Must be between 1 and 20")]
        public int Capacity { get; set; }

        public TableStatus Status { get; set; } = TableStatus.Available;

        [ValidateNever]
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
