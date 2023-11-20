using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace RaceCalendar.Domain.Services
{
    public class MailSender : IMailSender
    {
        public void SendRequest(RaceRequest request)
        {
            var subject = $"New Request - {(string.IsNullOrEmpty(request.NameId) ? "Ново" : "Промяна")}";
            var body = $@"
Тип: {((RaceTypes)request.Type).ToString()}
Име: {request.NameId}
Стара Дата: {request.OldStartDate}
Нова Дата: {request.StartDate}
Текст: {request.Text}
Източник: {request.Source}
Създадено на: {request.CreatedOn}

https://racecalendar.bg/requests
            ";

            using (var _client = CreateClient())
            {
                _client.Send(
                    "racecalendarbg@gmail.com",
                    "racecalendarbg@gmail.com",
                    subject,
                    body);
            }
        }

        public async Task<bool> SendAsync(string email, string subject, string body)
        {
            try
            {
                using (var _client = CreateClient())
                {
                    var message = new MailMessage();
                    message.From = new MailAddress("racecalendarbg@gmail.com");
                    message.To.Add(email);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    await _client.SendMailAsync(message);

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private SmtpClient CreateClient()
        {
            return new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("racecalendarbg@gmail.com", "rtmwactpkzqawkon")
            };
        }
    }
}
