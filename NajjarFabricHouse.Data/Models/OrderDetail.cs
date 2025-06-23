using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }

        [Required(ErrorMessage = "Order ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Order ID must be a positive integer.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit price is required.")]
        [Range(0.001, 10000, ErrorMessage = "Unit price must be between 0.001 and 10,000.")]
        [Column(TypeName = "decimal(10,3)")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Total price is required.")]
        [Range(0.001, 10000, ErrorMessage = "Total price must be between 0.001 and 10,000.")]
        [Column(TypeName = "decimal(10,3)")]
        public decimal TotalPrice { get; set; }
     
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
