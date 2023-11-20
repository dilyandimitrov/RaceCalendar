using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Services.Interfaces
{
    public interface IMailSender
    {
        void SendRequest(RaceRequest request);
        Task<bool> SendAsync(string email, string subject, string body);
    }
}
