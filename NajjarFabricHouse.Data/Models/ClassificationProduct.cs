using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("ClassificationProduct")]
    public class ClassificationProduct
    {
        public int ProductId { get; set; } // المفتاح الأجنبي للمنتج
        public Product Product { get; set; } // العلاقة مع المنتج

        public int CategoryId { get; set; } // المفتاح الأجنبي للتصنيف
        public Classification Classification { get; set; }
    }
}
