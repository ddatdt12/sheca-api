namespace Sheca.Services.Auth
{
    public interface IAuthService
    {
        public bool Login(string email, string password);
    }
}
