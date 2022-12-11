using System.ComponentModel.DataAnnotations;

namespace RaceCalendar.Domain.Models.Authentication
{
    public class ConfirmEmailRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
    }
}