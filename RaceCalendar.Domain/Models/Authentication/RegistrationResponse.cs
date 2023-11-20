namespace RaceCalendar.Domain.Models.Authentication
{
    public class RegistrationResponse : AuthResult
    {
        public bool IsSendConfirmationSucceeded { get; set; }
    }
}