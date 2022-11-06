using CliverApi.Services.Mail;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Sheca.Dtos;
using Sheca.Models;

namespace Sheca.Services
{
    public class EmailJob
    {
        private readonly ILogger<EmailJob> logger;
        private readonly IEventService _eventService;
        private readonly IMailService _emailService;

        public EmailJob(ILogger<EmailJob> logger, IEventService eventService, IMailService emailService)
        {
            this.logger = logger;
            _eventService = eventService;
            _emailService = emailService;
        }

        public async Task SendEmail()
        {
            var events = await _eventService.Get(new FilterEvent { FromDate = new DateTime(2022, 10, 30), ToDate = new DateTime(2022, 11, 30) });

            logger.LogInformation($"Greeting from Email Job {DateTime.Now} {string.Join("; ", events.Select(e => e.Title))}");
            BackgroundJob.Schedule(() => _emailService.SendEmail("ddatdt05@gmail.com", "Notifications", $"{string.Join("; ", events.Select(e => e.Title))} - {DateTime.Now}"), DateTime.Now.AddMinutes(1));
        }
    }
}
