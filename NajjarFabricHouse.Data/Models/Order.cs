using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{

    [Table("Orders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive integer.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Order date is required.")]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "DateTime")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Order status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        [RegularExpression(@"^(Pending|Shipped|Delivered)$", ErrorMessage = "Status must be 'Pending', 'Shipped', or 'Delivered'.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, 10000, ErrorMessage = "Total amount must be between 0.01 and 10,000.")]
        [Column(TypeName = "Decimal(10,3)")]
        public   decimal TotalAmount { get; set; }

        public Invoice Invoice { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public Customer Customer { get; set; }
    }
}
