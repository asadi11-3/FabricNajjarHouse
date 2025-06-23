using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NajjarFabricHouse.Service.Models.Authentication.User
{
    public class LoginOtpResponse
    {
        public string Token { get; set; } = null!;
        public bool TwoFactorEnable { get; set; }
    }
}
