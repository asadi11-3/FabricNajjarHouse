using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        [ForeignKey("Customer")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive integer.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        [ForeignKey("Product")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
        [Column(TypeName = "int")]
         public int Quantity { get; set; }

        [Required(ErrorMessage = "Added date is required.")]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "DateTime")]
        public DateTime AddedDate { get; set; } = DateTime.Now;

     
        public virtual Customer User { get; set; }
        public virtual Product Product { get; set; }
    }
}
