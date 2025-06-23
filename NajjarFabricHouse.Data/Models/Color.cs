using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("Color")]
    public class Color
    {
        [Required]
        [Column(TypeName ="NVARCHAR(50)")]
        public string ColorName { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ColorId { get; set;}
       public List<ProductVariant> ProductItems { get; set; }
      
    }
}
