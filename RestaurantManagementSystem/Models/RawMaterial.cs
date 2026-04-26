using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models
{
    public class RawMaterial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = "kg";

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentStock { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReorderLevel { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LastPurchasePrice { get; set; }

        public DateTime? LastPurchasedOn { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
