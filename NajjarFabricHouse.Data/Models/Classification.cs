using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("Classification")]
    public class Classification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassificationyId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        [Column(TypeName = "NVARCHAR(100)")]
        public string Categoryame { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Column(TypeName = "NVARCHAR(500)")]
        public string Description { get; set; }

        public List<ClassificationProduct> ClassificationProducts; // العلاقة مع المنتجات
      
    }

}
