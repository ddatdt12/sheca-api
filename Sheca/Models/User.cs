using System.ComponentModel.DataAnnotations.Schema;

namespace Sheca.Models
{
    [Table("User")]
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();
            Email = string.Empty;
            Password = string.Empty;
        }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}