using System.ComponentModel.DataAnnotations;

namespace Sheca.Dtos.User
{
    public class EmailUserDto
    {
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public EmailUserDto()
        {
            Email = string.Empty;
        }
    }
}
