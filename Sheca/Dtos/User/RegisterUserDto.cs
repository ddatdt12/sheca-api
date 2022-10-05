using System.ComponentModel.DataAnnotations;

namespace Sheca.Dtos
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Password is required and must be at least 6 character")]
        public string Password { get; set; }
        public RegisterUserDto()
        {
            Email = string.Empty;
            Password = string.Empty;
        }
    }
}
