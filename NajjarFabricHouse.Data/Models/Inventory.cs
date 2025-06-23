using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("Inventory")]
    public class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column(TypeName ="int")]
        public int InventoryId { get; set; }
        [ForeignKey("ProductVariant")]
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        [Column(TypeName = "int")]
        public int StockQuantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
