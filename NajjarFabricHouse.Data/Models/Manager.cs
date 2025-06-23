using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using NajjarFabricHouse.Data;

namespace NajjarFabricHouse.Data.Models
{
    [Table("Manager")]
    public class Manager
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int ManegerId { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Column(TypeName = "NVARCHAR(50)")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Column(TypeName = "NVARCHAR(50)")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [Column(TypeName = "NVARCHAR(100)")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        [Column(TypeName = "NVARCHAR(15)")]
        public string PhoneNumber { get; set; }
        public List<Employee> Employees { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
