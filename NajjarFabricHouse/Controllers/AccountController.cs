
using System.Text;

using NajjarFabricHouse.Dto;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NajjarFabricHouse.ViewModel;

namespace NajjarFabricHouse.Controllers
{
 
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;




      

        public AccountController()
        {
         
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7158"),
                Timeout = TimeSpan.FromMinutes(5)
            };
        }

        [HttpGet]
        public IActionResult AccountPerson()
        {
            return View(new AccountDto());
        }

        [HttpPost]
        public async Task<IActionResult> AccountPerson(AccountDto model)
        {
            if (model.ActionType == "Login")
            {
                return await HandleLoginAsync(model);
            }
            else if (model.ActionType == "Register")
            {
                return await HandleRegistrationAsync(model);
            }

            ModelState.AddModelError("", "Invalid action type.");
            return View(model);
        }
        [HttpGet]
      public async Task<IActionResult> Logout()
        {
            var response = await _httpClient.PostAsync("api/Authentication/Logout", null);
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Logout successful!";
                return RedirectToAction("Index", "Home");
            }
            TempData["Error"] = "An error occurred while logging out.";
            return RedirectToAction("AccountPerson", "Account");
        }

        private async Task<IActionResult> HandleLoginAsync(AccountDto model)
        {
            if (string.IsNullOrEmpty(model.LoginUserName) || string.IsNullOrEmpty(model.LoginPassword))
            {
                ModelState.AddModelError("", "Username and Password are required for login.");
                return View("AccountPerson", model);
            }

            var loginData = new
            {
                UserName = model.LoginUserName,
                Password = model.LoginPassword
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Authentication/Login", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Login successful!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt. Please check your credentials.");
            return View("AccountPerson", model);
        }

        private async Task<IActionResult> HandleRegistrationAsync(AccountDto model)
        {
            if (string.IsNullOrEmpty(model.RegisterUserName) || string.IsNullOrEmpty(model.RegisterEmail) || string.IsNullOrEmpty(model.RegisterPassword))
            {
                ModelState.AddModelError("", "All registration fields are required.");
                return View("AccountPerson", model);
            }

            var registerData = new
            {
                UserName = model.RegisterUserName,
                Email = model.RegisterEmail,
                Password = model.RegisterPassword,
                Roles = new[] { "User" }
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Authentication/Register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Registration successful!";
                return RedirectToAction("Index", "Home");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Registration failed: {errorMessage}");
            return View("AccountPerson", model); 
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View("ForgotPassword", model);
            }

            try
            {
                // استدعاء API لإرسال رابط إعادة تعيين كلمة المرور
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Authentication/ForgotPassword", content);

                if (response.IsSuccessStatusCode)
                {
                    // قراءة الاستجابة وتحويلها إلى JSON
                    var responseData = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData);

                    if (jsonResponse.ContainsKey("data"))
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse["data"].ToString());
                        TempData["Email"] = data["email"];
                        TempData["Token"] = data["token"];
                        return RedirectToAction("ResetPassword");
                    }
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View("ForgotPassword", model);
        }




        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (TempData["Email"] == null || TempData["Token"] == null)
            {
                TempData["Error"] = "Email or token is missing.";
                return RedirectToAction("AccessDenied");
            }

            var model = new ResetPasswordDto
            {
                Email = TempData["Email"].ToString(),
                Token = TempData["Token"].ToString()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid input.");
                return View("ResetPassword", model);
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Authentication/ResetPassword", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Password reset successfully!";
                return RedirectToAction("AccountPerson");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Error: {errorMessage}");
            return View("ResetPassword", model);
        }
      
        [HttpGet("ResetPassword")]
        public IActionResult ShowResetPasswordPage(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
               
                TempData["Error"] = "Invalid email or token.";
                return RedirectToAction("AccessDenied"); 
            }

            // تمرير البيانات إلى الصفحة
            var model = new ResetPasswordDto
            {
                Email = email,
                Token = token
            };

            return View("ResetPasswordPage", model); // عرض صفحة ResetPasswordPage
        }
       





        #region AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion
    }
}
