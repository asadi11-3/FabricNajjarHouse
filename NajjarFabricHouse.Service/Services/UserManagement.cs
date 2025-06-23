using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NajjarFabricHouse.API.Models;
using NajjarFabricHouse.Data;
using NajjarFabricHouse.Data.Models;
using NajjarFabricHouse.Service.Models;
using NajjarFabricHouse.Service.Models.Authentication.Login;
using NajjarFabricHouse.Service.Models.Authentication.SignUp;
using NajjarFabricHouse.Service.Models.Authentication.SignUp.User;
using NajjarFabricHouse.Service.Models.Authentication.User;
using NajjarFabricHouse.Service.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace User.Management.Service.Services
{
    public class UserManagement : IUserManagement
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailServices _emailService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagement(UserManager<ApplicationUser> userManager,
                              IConfiguration configuration,
                              IEmailServices emailService,
                              SignInManager<ApplicationUser> signInManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        #region User Management Methods

        public async Task<ApiResponse<string>> CreateUserWithTokenAsync(RegisterUser registerUser)
        {
            if (registerUser == null)
            {
                return new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Invalid input data",
                    StatusCode = 400
                };
            }

            var userExists = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExists != null)
            {
                return new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "User already exists!",
                    StatusCode = 409
                };
            }

            var user = new ApplicationUser
            {
                UserName = registerUser.UserName,
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                return new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Failed to create user!",
                    StatusCode = 500
                };
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "User created successfully!",
                Response = token,
                StatusCode = 201
            };
        }

        public async Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user)
        {
            if (user == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "User not found!",
                    StatusCode = 404
                };
            }

          
            var authClaims = await GenerateClaimsAsync(user);

          
            var jwtToken = GenerateJwtToken(authClaims);

           
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new ApiResponse<LoginResponse>
            {
                IsSuccess = true,
                Message = "JWT Token generated successfully!",
                StatusCode = 200,
                Response = new LoginResponse
                {
                    AccessToken = new JwtToken()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        Expiration = jwtToken.ValidTo,
                    },
                    RefreshToken = new JwtToken()
                    {
                        Token = refreshToken,
                        Expiration = user.RefreshTokenExpiry ?? DateTime.UtcNow.AddDays(7)
                    }
                }
            };
        }
        public async Task<ApiResponse<LoginResponse>> LogoutUserAsync(ApplicationUser user)
        {
            if (user == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "User not found!",
                    StatusCode = 404
                };
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

          
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    Message = $"Failed to logout user: {errors}",
                    StatusCode = 500
                };
            }

            return new ApiResponse<LoginResponse>
            {
                IsSuccess = true,
                Message = "User logged out successfully!",
                StatusCode = 200
            };
        }




        public async Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.UserName);

            if (user == null)
            {
                return new ApiResponse<LoginOtpResponse>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "User not found."
                };
            }

            if (!user.TwoFactorEnabled)
            {
                return new ApiResponse<LoginOtpResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Two-Factor Authentication is not enabled for this user."
                };
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var message = new Message(new[] { user.Email! }, "OTP Login", $"Your OTP code is: {token}");
            _emailService.SendEmail(message);

            return new ApiResponse<LoginOtpResponse>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = $"OTP sent to email {user.Email}",
                Response = new LoginOtpResponse { Token = token, TwoFactorEnable = user.TwoFactorEnabled }
            };
        }

        public async Task<ApiResponse<LoginResponse>> LoginUserWithJWtTokenAsync(string otp, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "User not found!",
                    StatusCode = 404
                };
            }

            var signInResult = await _signInManager.TwoFactorSignInAsync("Email", otp, false, false);
            if (signInResult.Succeeded)
            {
                return await GetJwtTokenAsync(user);
            }

            return new ApiResponse<LoginResponse>
            {
                IsSuccess = false,
                Message = "Invalid OTP or login failed.",
                StatusCode = 401
            };
        }
        public async Task<ApiResponse<List<string>>> AssignRoleToUserAsync(IEnumerable<string> roles, ApplicationUser user)
        {
            if (user == null)
            {
                return new ApiResponse<List<string>>
                {
                    IsSuccess = false,
                    Message = "User not found!",
                    StatusCode = 404
                };
            }

            var assignedRoles = new List<string>();

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return new ApiResponse<List<string>>
                    {
                        IsSuccess = false,
                        Message = $"Role '{role}' does not exist!",
                        StatusCode = 400
                    };
                }

                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    var result = await _userManager.AddToRoleAsync(user, role);
                    if (result.Succeeded)
                    {
                        assignedRoles.Add(role);
                    }
                    else
                    {
                        return new ApiResponse<List<string>>
                        {
                            IsSuccess = false,
                            Message = $"Failed to assign role '{role}' to user!",
                            StatusCode = 500
                        };
                    }
                }
            }

            return new ApiResponse<List<string>>
            {
                IsSuccess = true,
                Message = "Roles assigned successfully!",
                StatusCode = 200,
                Response = assignedRoles
            };
        }
        public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(LoginResponse tokens)
        {
            var accessToken = tokens.AccessToken;
            var refreshToken = tokens.RefreshToken;

           
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);

            try
            {
                var principal = tokenHandler.ValidateToken(accessToken.Token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, 
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

              
                var username = principal.Identity?.Name;
                if (username == null)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid token!",
                        StatusCode = 401
                    };
                }

               
                var user = await _userManager.FindByNameAsync(username);
                if (user == null || user.RefreshToken != refreshToken.Token || user.RefreshTokenExpiry <= DateTime.UtcNow)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid refresh token!",
                        StatusCode = 401
                    };
                }

            
                var authClaims = await GenerateClaimsAsync(user);
                var newAccessToken = GenerateJwtToken(authClaims);

            
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = true,
                    Message = "Token refreshed successfully!",
                    StatusCode = 200,
                    Response = new LoginResponse
                    {
                        AccessToken = new JwtToken
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                            Expiration = newAccessToken.ValidTo
                        },
                        RefreshToken = new JwtToken
                        {
                            Token = refreshToken.Token, 
                            Expiration = user.RefreshTokenExpiry ?? DateTime.UtcNow.AddDays(7)
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "Invalid token or refresh token!",
                    StatusCode = 401
                };
            }
        }
        public async Task<ApiResponse<LoginResponse>> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
         
            if (user == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "User not found!",
                    StatusCode = 404
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            
            if (!result.Succeeded)
            {
               
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    Message = $"Failed to change password: {errors}",
                    StatusCode = 400
                };
            }

          
            return new ApiResponse<LoginResponse>
            {
                IsSuccess = true,
                Message = "Password changed successfully!",
                StatusCode = 200
            };
        }
        public async Task<ApiResponse<LoginResponse>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var response = new ApiResponse<LoginResponse>();

            if (string.IsNullOrEmpty(forgotPasswordDto.Email))
            {
                response.IsSuccess = false;
                response.Message = "Email is required.";
                response.StatusCode = 400;
                return response;
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "User not found!";
                response.StatusCode = 404;
                return response;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://localhost:5280//reset-password?email={user.Email}&token={resetToken}";

            var message = new Message(new[] { user.Email! }, "Password Reset Request",
                $"Please use the following link to reset your password: {resetLink}");
            _emailService.SendEmail(message);

            response.IsSuccess = true;
            response.Message = "Password reset link sent successfully.";
            response.StatusCode = 200;

            return response;
        }
        public async Task<ApiResponse<LoginResponse>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var response = new ApiResponse<LoginResponse>();

          
            if (string.IsNullOrEmpty(resetPasswordDto.Email) || string.IsNullOrEmpty(resetPasswordDto.Token) || string.IsNullOrEmpty(resetPasswordDto.NewPassword))
            {
                response.IsSuccess = false;
                response.Message = "All fields are required.";
                response.StatusCode = 400;
                return response;
            }

         
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "User not found!";
                response.StatusCode = 404;
                return response;
            }

            
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
              
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                response.IsSuccess = false;
                response.Message = $"Password reset failed: {errors}";
                response.StatusCode = 400;
                return response;
            }

            response.IsSuccess = true;
            response.Message = "Password has been reset successfully!";
            response.StatusCode = 200;

            return response;
        }




        #endregion

        #region Private Helper Methods

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
            _ = int.TryParse(_configuration["JwtSettings:tokenValidityMinutes"], out int tokenValidityInMinutes);

            return new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

      











        #endregion
    }
}
