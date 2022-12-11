using System.ComponentModel.DataAnnotations;

namespace RaceCalendar.Domain.Models.Authentication
{
    public class UpdateUserRequest
    {
        [Required]
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}