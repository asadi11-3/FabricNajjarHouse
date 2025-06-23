using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NajjarFabricHouse.Service.Models.Authentication.SignUp.User
{
    public class ForgotPasswordDto
    {
        [EmailAddress]
        [Required]
        public string? Email { get; set; }
    }
}
