using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NajjarFabricHouse.Data.Models
{
    public   class ApplicationUser: IdentityUser
    {
        public string? RefreshToken { get; set; } = null!;
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
