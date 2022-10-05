using System.ComponentModel.DataAnnotations;

namespace Sheca.Dtos.User
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required, MinLength(6, ErrorMessage = "Password is required and must be at least 6 character")]
        public string Password { get; set; }
        [Required, MinLength(6, ErrorMessage = "New password is required and must be at least 6 character")]
        public string NewPassword { get; set; }
    }
}
