using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NajjarFabricHouse.Data;
using NajjarFabricHouse.Data.Models;

namespace NajjarFabricHouse.Service.Models.Authentication.User
{
    public  class CreateUserResponse
    {

        public string? Token { get; set; }
        public ApplicationUser User { get; set; } = null!;

    }
}
