using System.ComponentModel.DataAnnotations;

namespace Sheca.Dtos.User
{
    public class TokenForgotPasswordDto
    {
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
