using System.ComponentModel.DataAnnotations;

namespace Sheca.Dtos.User
{
    public class TokenResetPasswordDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters, dude!")]
        public string Password { get; set; } = string.Empty;
    }
}
