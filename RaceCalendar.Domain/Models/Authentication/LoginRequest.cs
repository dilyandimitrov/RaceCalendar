using System.ComponentModel.DataAnnotations;

namespace RaceCalendar.Domain.Models.Authentication
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}