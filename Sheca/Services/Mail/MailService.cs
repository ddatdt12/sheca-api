using CliverApi.Services.Mail;
using System.Net;
using System.Net.Mail;

namespace Sheca.Services.Mail
{
    public class MailService : IMailService
    {
        public async Task<String> SendRegisterMail(string email)
        {
            string code = new Random().Next(1000, 9999).ToString();
            await Task.Run(() =>
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("Shecaapp@gmail.com");
                mail.Subject = "ShecaApp - Verification mail";
                mail.Body = "This is your verification code: " + code;
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = new NetworkCredential("Shecaapp@gmail.com", "lfrtpskqnkxqczhl");
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
                mail.From = new MailAddress("Shecaapp@gmail.com");
                mail.Subject = "ShecaApp - Account recovery code mail";
                mail.Body = "Enter the following password reset code: " + code;
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = new NetworkCredential("Shecaapp@gmail.com", "lfrtpskqnkxqczhl");
                smtp.Send(mail);
            });
            return code;
        }
    }
}
