using System.ComponentModel.DataAnnotations;

namespace Sheca.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public UserDto()
        {
            Email = string.Empty;
        }
    }
}
