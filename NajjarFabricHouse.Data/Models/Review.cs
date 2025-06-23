using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("Review")]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        [ForeignKey("Product")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        [ForeignKey("Customer")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive integer.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }
        [Column(TypeName = "int")]
        public int ProductClassificationId { get; set; }
        [Required(ErrorMessage = "Review date is required.")]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "DateTime")]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

  
      
        public virtual Product Product { get; set; }
        public virtual Customer User { get; set; }
       
    }
}
