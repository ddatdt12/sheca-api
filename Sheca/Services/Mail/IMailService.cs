﻿using System.Threading.Tasks;

namespace CliverApi.Services.Mail
{
    public interface IMailService
    {
        public Task<String> SendRegisterMail(string email);
    }
}