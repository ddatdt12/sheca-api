namespace Sheca.Models
{
    public class Token
    {
        public string Value { get; set; } = null!;
        public DateTime ExpiredAt;
        public User User = null!;
    }
}
