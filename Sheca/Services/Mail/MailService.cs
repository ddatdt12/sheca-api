using CliverApi.Services.Mail;
using System.Net;
using System.Net.Mail;

namespace Sheca.Services.Mail
{
    public class MailService : IMailService
    {
        ICredentialsByHost _credentialsByHost;
        public MailService()
        {
            _credentialsByHost = new NetworkCredential("gahulla2002@gmail.com", "tiyjilkneqakfxkh"); 
        }
        public async Task<String> SendRegisterMail(string email)
        {
            string code = new Random().Next(1000, 9999).ToString();
            await Task.Run(() =>
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("gahulla2002@gmail.com");
                mail.Subject = "ShecaApp - Verification mail";
                mail.Body = "This is your verification code: " + code;
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = _credentialsByHost;
                smtp.Send(mail);
            });
            return code;
        }

        public async Task<String> SendResetPasswordMail(string email)
        {
            string code = new Random().Next(1000, 9999).ToString();
            await Task.Run(() =>
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("gahulla2002@gmail.com");
                mail.Subject = "ShecaApp - Account recovery code mail";
                mail.Body = "Enter the following password reset code: " + code;
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = _credentialsByHost;
                smtp.Send(mail);
            });
            return code;
        }

        public async Task SendEmail(string email, string subject, string body)
        {
            string code = new Random().Next(1000, 9999).ToString();
            await Task.Run(() =>
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("gahulla2002@gmail.com");
                mail.Subject = subject;
                mail.Body = body;
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = _credentialsByHost;
                smtp.Send(mail);
            });
        }
    }
}
