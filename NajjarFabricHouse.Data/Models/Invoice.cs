using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("Invoices")]
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Order ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Order ID must be a positive integer.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Invoice date is required.")]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "DateTime")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, 10000, ErrorMessage = "Total amount must be between 0.01 and 10,000.")]
        [Column(TypeName = "Decimal(10,3)")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Payment status is required.")]
        [StringLength(50, ErrorMessage = "Payment status cannot exceed 50 characters.")]
        [RegularExpression(@"^(Paid|Unpaid)$", ErrorMessage = "Payment status must be either 'Paid' or 'Unpaid'.")]
        [Column(TypeName = "NVARCHAR(50)")]
        public string PaymentStatus { get; set; }
        [Column(TypeName ="int")]
        public int ManagerId { get; set; }
        public Order Order { get; set; }
      
    }
}
