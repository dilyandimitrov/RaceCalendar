using System.ComponentModel.DataAnnotations;

namespace RaceCalendar.Domain.Models.Authentication
{
    public class UserResponse
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
    }
}