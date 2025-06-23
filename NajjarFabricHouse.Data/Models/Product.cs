
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace NajjarFabricHouse.Data.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Column(TypeName = "NVARCHAR(100)")]
        public string NameProduct { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000.")]
        [Column(TypeName = "Decimal(10,3)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Pattern is required.")]
        [StringLength(50, ErrorMessage = "Pattern cannot exceed 50 characters.")]
        [Column(TypeName = "NVARCHAR(50)")]
        public string Pattern { get; set; }

       

        [Required]
        [Column(TypeName = "DateTime")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

       
       

        [Column(TypeName = "bit")]
        [Required]
        public bool IsDeleted { get; set; }

        public List<ClassificationProduct> ClassificationProducts { get; set; }
        public List<ProductVariant> ProductVariant { get; set; }
        public List<CartItem> CartItems { get; set; }
        public List<Review> Reviews { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
      
    }

}
