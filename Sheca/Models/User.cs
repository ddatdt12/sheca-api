namespace Sheca.Models
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();
            Email = string.Empty;
            Password = string.Empty;
            Name = string.Empty;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

    }
}