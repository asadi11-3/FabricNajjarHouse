using Microsoft.AspNetCore.Identity;
using NajjarFabricHouse.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NajjarFabricHouse.Data.Models
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column(TypeName = "int")]
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Column(TypeName = "NVARCHAR(50)")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [Column(TypeName = "NVARCHAR(100)")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Column(TypeName = "NVARCHAR(200)")]
        public string? PasswordHash { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Address cannot exceed 50 characters.")]
        [Column(TypeName = "NVARCHAR(50)")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        [Column(TypeName = "NVARCHAR(15)")]
        public string? PhoneNumber { get; set; }
        public List<string>? Roles { get; set; }

        [Required]
        [Column(TypeName = "DateTime")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<Order> Orders { get; set; }
        public List<CartItem> CartItems { get; set; }
        public List<Review>? Reviews { get; set; }
       
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
