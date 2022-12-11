using System.ComponentModel.DataAnnotations;

namespace RaceCalendar.Domain.Models.Authentication
{
    public class SendEmailConfirmationRequest
    {
        [Required(ErrorMessage = "Възникна грешка.")]
        public string Email { get; set; }
    }
}