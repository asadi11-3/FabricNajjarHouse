using Microsoft.AspNetCore.Identity;
using NajjarFabricHouse.Data.Models;
using NajjarFabricHouse.Service.Models;
using NajjarFabricHouse.Service.Models.Authentication.Login;
using NajjarFabricHouse.Service.Models.Authentication.SignUp;
using NajjarFabricHouse.Service.Models.Authentication.SignUp.User;
using NajjarFabricHouse.Service.Models.Authentication.User;



namespace NajjarFabricHouse.API.Models
{
    public interface IUserManagement
    {
        Task<ApiResponse<string>> CreateUserWithTokenAsync(RegisterUser registerUser);
        Task<ApiResponse<List<string>>> AssignRoleToUserAsync(IEnumerable<string> roles,ApplicationUser user);
        Task<ApiResponse<LoginOtpResponse>>GetOtpByLoginAsync(LoginModel loginModel   );
        Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user);
        Task<ApiResponse<LoginResponse>> LoginUserWithJWtTokenAsync(string otp, string userName);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(LoginResponse loginResponse);
        Task<ApiResponse<LoginResponse>> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<ApiResponse<LoginResponse>> LogoutUserAsync(ApplicationUser user);
        Task<ApiResponse<LoginResponse>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<ApiResponse<LoginResponse>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
      

    }

}
