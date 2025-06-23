using System.ComponentModel.DataAnnotations;

namespace NajjarFabricHouse.Service.Models.Authentication.SignUp
{
    public class ResetPassword
    {
        [Required]
        public string Password { get; set; } = null!;
        [Compare("Password",ErrorMessage ="The Password and Confirmation password do not match")]
        public string ConfirmPassword { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
