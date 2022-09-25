namespace Sheca.Dtos
{
    public class UserDTO
    {
        public UserDTO()
        {
            Email = string.Empty;
            Password = string.Empty;
        }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
