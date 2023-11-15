using RaceCalendar.Domain.Models.Authentication;
using RaceCalendar.Domain.Responses;

namespace RaceCalendar.Domain.Services.Interfaces
{
    public interface IUserService
    {
        Task<RegistrationResponse> RegisterUserAsync(RegistrationRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RegistrationResponse> SendEmailConfirmation(SendEmailConfirmationRequest request);
        Task<bool> ConfirmEmail(ConfirmEmailRequest request);

        Task<IEnumerable<GetAllUsersResponse>> GetAll();
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(string id);
        Task<AuthResult> Update(UpdateUserRequest request);
        Task<AuthResult> Delete(string email);
    }
}
