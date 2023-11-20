namespace RaceCalendar.Domain.Models.Authentication
{
    public class LoginResponse : AuthResult
    {
        public bool? IsEmailConfirmed { get; set; }
    }
}