using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RestaurantManagementSystem.Models
{
    public enum OrderStatus
    {
        Pending, Confirmed, Preparing, Ready, Sent, Completed, Cancelled
    }
    public enum ServiceType
    {
        DineIn, Takeaway, Delivery
    }
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public int? TableId { get; set; }

        [ForeignKey("TableId")]
        [ValidateNever]
        public virtual Table? Table { get; set; }

        [Required]
        public ServiceType ServiceType { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [ValidateNever]
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
