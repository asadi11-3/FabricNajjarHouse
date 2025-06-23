
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NajjarFabricHouse.Data;
using NajjarFabricHouse.Data.Models;
using NajjarFabricHouse.Service.Models;
using NajjarFabricHouse.Service.Models.Authentication.Login;
using NajjarFabricHouse.Service.Models.Authentication.SignUp;
using NajjarFabricHouse.Service.Models.Authentication.SignUp.User;
using NajjarFabricHouse.Service.Models.Authentication.User;
using NajjarFabricHouse.Service.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using User.Management.API.Models;


namespace NajjarFabricHouse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailServices _emailService;
        private readonly IConfiguration _configuration;


        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            IEmailServices emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        #region Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
           
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
                return Conflict(new Response { Status = "Error", Message = "User already exists!" });

       
            var user = new ApplicationUser
            {
                Email = registerUser.Email,
                UserName = registerUser.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = true,
                RefreshToken = GenerateRefreshToken(), 
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7) 
            };

           
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Failed to create user!" });

            
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);

           
            var message = new Message(new[] { user.Email! }, "Confirmation email link", confirmationLink!);
            _emailService.SendEmail(message);
            await _userManager.AddToRoleAsync(user, "User");
         
            return Ok(new Response
            {
                Status = "Success",
                Message = "User registered successfully! Please confirm your email."
            });
        }

        #endregion

        #region ConfirmEmail
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new Response { Status = "Error", Message = "User not found!" });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Failed to verify email!" });

            return Ok(new Response { Status = "Success", Message = "Email verified successfully!" });
        }
        #endregion

        #region Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
           
            var user = await _userManager.FindByNameAsync(loginModel.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
                return Unauthorized(new Response { Status = "Error", Message = "Invalid credentials!" });


            if (user.TwoFactorEnabled)
            {
                var otpToken = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                var message = new Message(new[] { user.Email! }, "OTP for Login", $"Your OTP code is: {otpToken}");
                _emailService.SendEmail(message);

                return Ok(new Response { Status = "Success", Message = "OTP has been sent to your email." });
            }

            var authClaims = await GenerateClaimsAsync(user);
            var jwtToken = GenerateJwtToken(authClaims);

          
            if (string.IsNullOrEmpty(user.RefreshToken) || user.RefreshTokenExpiry == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                user.RefreshToken = GenerateRefreshToken();
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return StatusCode(500, new { Message = $"Failed to update user: {errors}" });
                }
            }

         
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo,
                refreshToken = user.RefreshToken,
                refreshTokenExpiration = user.RefreshTokenExpiry
            });
        }



        #endregion
        #region Logout
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
           
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User is not authenticated!" });

            
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound(new { Message = "User not found!" });

          
            user.RefreshToken = null;
            user.RefreshTokenExpiry = DateTime.UtcNow;

         
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(500, new { Message = $"Failed to logout user: {errors}" });
            }

            return Ok(new { Message = "User logged out successfully!" });
        }
        #endregion



        #region LoginWithOTP
        [HttpPost("Login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string username, string otp)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound(new Response { Status = "Error", Message = "User not found!" });

            var isOtpValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", otp);
            if (!isOtpValid)
                return Unauthorized(new Response { Status = "Error", Message = "Invalid OTP!" });

         
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return StatusCode(500, new { Message = $"Failed to confirm email: {errors}" });
                }
            }

           
            var authClaims = await GenerateClaimsAsync(user);
            var jwtToken = GenerateJwtToken(authClaims);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo,
                refreshToken = refreshToken,
                message = "Email has been confirmed successfully and user logged in."
            });
        }

        #endregion
        #region ChangePassword
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found!" });
            }


            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { Message = $"Failed to change password: {errors}" });
            }

            return Ok(new { Message = "Password changed successfully!" });
        }
        #endregion


        #region RefreshToken
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] LoginResponse tokens)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);

            try
            {
                var principal = tokenHandler.ValidateToken(tokens.AccessToken.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out _);

                var username = principal.Identity?.Name;
                var user = await _userManager.FindByNameAsync(username);
                if (user == null || user.RefreshToken != tokens.RefreshToken.Token || user.RefreshTokenExpiry <= DateTime.UtcNow)
                    return Unauthorized(new { Message = "Invalid refresh token or token expired!" });

                var authClaims = await GenerateClaimsAsync(user);
                var newAccessToken = GenerateJwtToken(authClaims);

                return Ok(new
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                    Expiration = newAccessToken.ValidTo,
                    RefreshToken = user.RefreshToken,
                    RefreshTokenExpiration = user.RefreshTokenExpiry
                });
            }
            catch
            {
                return Unauthorized(new { Message = "Invalid token or refresh token!" });
            }
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (string.IsNullOrEmpty(forgotPasswordDto.Email))
            {
                return BadRequest(new { Message = "Email is required." });
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new { Message = "User not found!" });
            }

          
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(resetToken);

            
            var resetLink = Url.Action(
                "ResetPassword",
                "Authentication",
                new { email = user.Email, token = encodedToken },
                Request.Scheme
            );

         
            var message = new Message(new[] { user.Email }, "Reset Password", $"Click the link to reset your password: {resetLink}");
            _emailService.SendEmail(message);

       
            return Ok(new
            {
                data = new
                {
                    email = user.Email,
                    token = resetToken 
                }
            });
        }



        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Message = "Invalid email or token." });
            }


            var model = new ResetPasswordDto
            {
                Email = email,
                Token = token
            };

            return Ok(model); 
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Invalid input data.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return NotFound(new { Message = "User not found!" });

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { Message = $"Failed to reset password: {errors}" });
            }

            return Ok(new { Message = "Password reset successfully!" });
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userDetails = new List<object>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDetails.Add(new
                    {
                        user.UserName,
                        user.Email,
                        Roles = roles.Any() ? roles : new List<string> { "No Roles" }
                    });
                }

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Details = ex.Message });
            }
        }











        #endregion
        #region Helpers
        private async Task<List<Claim>> GenerateClaimsAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? "UnknownUser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }



        private JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            return new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.UtcNow.AddMinutes(5),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        #endregion
    }

}