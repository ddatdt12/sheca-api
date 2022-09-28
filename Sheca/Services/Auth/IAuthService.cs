namespace Sheca.Services
{
    public interface IAuthService
    {
        public bool Login(string email, string password);
    }
}
