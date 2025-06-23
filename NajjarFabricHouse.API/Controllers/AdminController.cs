using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NajjarFabricHouse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("AdminController")]
        public IEnumerable<String> GetStrings() {


            return new List<String> { "Ail", "Samer" };
        }
    }
}
