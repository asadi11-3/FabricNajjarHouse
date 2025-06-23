using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NajjarFabricHouse.Dto
{
    public class AccountDto : IValidatableObject
    {
        public string? ActionType { get; set; }

       
        public string? LoginUserName { get; set; }
        public string? LoginPassword { get; set; }

    
        public string? RegisterUserName { get; set; }
        public string? RegisterEmail { get; set; }
        public string? RegisterPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ActionType == "Login")
            {
                if (string.IsNullOrEmpty(LoginUserName))
                    yield return new ValidationResult("Username is required for login.", new[] { nameof(LoginUserName) });

                if (string.IsNullOrEmpty(LoginPassword))
                    yield return new ValidationResult("Password is required for login.", new[] { nameof(LoginPassword) });
            }
            else if (ActionType == "Register")
            {
                if (string.IsNullOrEmpty(RegisterUserName))
                    yield return new ValidationResult("Username is required for registration.", new[] { nameof(RegisterUserName) });

                if (string.IsNullOrEmpty(RegisterEmail))
                    yield return new ValidationResult("Email is required for registration.", new[] { nameof(RegisterEmail) });

                if (string.IsNullOrEmpty(RegisterPassword))
                    yield return new ValidationResult("Password is required for registration.", new[] { nameof(RegisterPassword) });
            }
        }


    }
}