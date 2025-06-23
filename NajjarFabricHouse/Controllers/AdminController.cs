

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NajjarFabricHouse.Data.DataBase;
using NajjarFabricHouse.Data.Models;
using NajjarFabricHouse.Models;
using System;
using System.Net.Http;

namespace NajjarFabricHouse.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private AppDbContext appDbContext { get; set; }
        public AdminController(AppDbContext appDbContext,HttpClient _httpClient) { 
        this.appDbContext=appDbContext;
           this._httpClient = new HttpClient
            {
                BaseAddress = new System.Uri("https://localhost:7158"),
                Timeout = TimeSpan.FromHours(24)
            };
        }
  
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userDtos = await GetUsersFromApi();

              
                var users = userDtos.Select(model => new NajjarFabricHouse.Dto.UserDto
                {
                  
                    UserName = model.UserName,
                    Email=model.Email,
                    Roles = model.Roles


                }).ToList();

                return View(users);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View("Error");
            }
        }



        [HttpGet]

        public IActionResult PageAdmin()
        {
          
            return View();
        }
        [HttpGet]
        public IActionResult Create() {

            return View();
        }
        [HttpPost]
        public IActionResult Create(string name) {

            return View();
        }
        public IActionResult UpdateRole() {

            return View();
        }
        public IActionResult TotalSalary() {
            return View();
        
        }
        public  async Task<List<Customer>> GetUsersFromApi()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/authentication/users");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        throw new Exception("API returned empty response.");
                    }

                    var users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer>>(json);
                    return users ?? new List<Customer>(); // Return empty list if deserialization results in null
                }
                else
                {
                    throw new Exception($"API returned error: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }



    }
}
