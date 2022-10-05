namespace Sheca.Models
{
    public class Token
    {
        public string Code { get; set; } = null!;
        public DateTime ExpiredAt;
        public User User = null!;
    }
}
