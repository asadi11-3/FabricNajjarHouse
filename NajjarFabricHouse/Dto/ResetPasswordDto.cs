namespace NajjarFabricHouse.Dto
{
   
    public class ResetPasswordDto
    {
        public string?   Email { get; set; }
        public string? Token { get; set; }
        public string? NewPassword { get; set; }


    }
    public class ForgotPasswordDto
    {
        public string? Email { get; set; }
    }
    public class TokenResponseDto
    {
        public string? Token { get; set; }
    }
}
