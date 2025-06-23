using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("ProductVariant")]
    public class ProductVariant
    {
        [Key]
        public int ProductVariantId { get; set; } // المفتاح الأساسي

        [ForeignKey("Product")]
        public int ProductId { get; set; } // العلاقة مع المنتج
        public Product Product { get; set; }

        [ForeignKey("Color")]
        public int ColorId { get; set; } // العلاقة مع اللون
        public Color Color { get; set; }

        public List<Inventory> Inventories { get; set; }
        [Required]
        [StringLength(200)]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string ImageUrl { get; set; }

        [StringLength(50)]
        public string ImageDescription { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Please enter a valid length between 1 and 1000.")]
        public double Length { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Please enter a valid width between 1 and 1000.")]
        public double Width { get; set; } // العرض بالمتر
    }
}
