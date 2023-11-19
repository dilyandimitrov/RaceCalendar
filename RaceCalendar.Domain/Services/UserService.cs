using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RaceCalendar.Domain.Models.Authentication;
using RaceCalendar.Domain.Services.Interfaces;
using RaceCalendar.Domain.Responses;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Commands;

namespace RaceCalendar.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IMailSender _mailSender;
        private readonly IGetAllUsersQuery _getAllUsersQuery;
        private readonly IGetUserRacesByUserQuery _getUserRacesByUserQuery;
        private readonly IDeleteUserRaceCommand _deleteUserRaceCommand;
        private readonly IDeleteUserSettingsCommand _deleteUserSettingsCommand;

        public UserService(
            IConfiguration config,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtOptions> jwtOptions,
            IMailSender mailSender,
            IGetAllUsersQuery getAllUsersQuery,
            IGetUserRacesByUserQuery getUserRacesByUserQuery,
            IDeleteUserRaceCommand deleteUserRaceCommand,
            IDeleteUserSettingsCommand deleteUserSettingsCommand)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _getAllUsersQuery = getAllUsersQuery ?? throw new ArgumentNullException(nameof(getAllUsersQuery));
            _getUserRacesByUserQuery = getUserRacesByUserQuery ?? throw new ArgumentNullException(nameof(getUserRacesByUserQuery));
            _deleteUserRaceCommand = deleteUserRaceCommand ?? throw new ArgumentNullException(nameof(deleteUserRaceCommand));
            _deleteUserSettingsCommand = deleteUserSettingsCommand ?? throw new ArgumentNullException(nameof(deleteUserSettingsCommand));
        }

        public async Task<IEnumerable<GetAllUsersResponse>> GetAll()
        {
            var users = (await _getAllUsersQuery.Get()).ToList();

            users.Sort((u1, u2) => DateTime.Compare(u2.CreatedOn, u1.CreatedOn));

            return users;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                user.IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            }

            return user;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            }

            return user;
        }

        public async Task<RegistrationResponse> RegisterUserAsync(RegistrationRequest request)
        {
            var result = new RegistrationResponse();

            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                result.Errors = new List<string>() { "Потребител с този еmail вече съществува" };
                return result;
            }

            var user = new User()
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedOn = DateTime.UtcNow
            };

            var userCreatedResult = await _userManager.CreateAsync(user, request.Password);

            if (userCreatedResult.Succeeded)
            {
                await MakeUsAdminsAsync(user);
                var isSendSucceeded = await SendEmailConfirmation(user);

                if (!isSendSucceeded)
                {
                    await _userManager.DeleteAsync(user);
                    result.Errors = new List<string>() { "Възникна грешка при пращане на кода за потвърждаване на електронната Ви поща." };
                    return result;
                }

                result.IsSendConfirmationSucceeded = true;
                result.Result = true;
                return result;
            }

            result.Errors = TranslateErrors(userCreatedResult.Errors);
            return result;
        }

        public async Task<RegistrationResponse> SendEmailConfirmation(SendEmailConfirmationRequest request)
        {
            var result = new RegistrationResponse();

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return result;
            }

            var isSendSucceeded = await SendEmailConfirmation(user);

            if (!isSendSucceeded)
            {
                result.Errors = new List<string>() { "Възникна грешка при пращане на email-ът за потвърждаване" };
                return result;
            }

            result.IsSendConfirmationSucceeded = true;
            return result;
        }

        public async Task<bool> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || string.IsNullOrEmpty(request.Token))
            {
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);

            return result.Succeeded;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var result = new LoginResponse();

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                result.IsEmailConfirmed = null;
                result.Errors = new List<string>() { "Грешен email или парола." };
                return result;
            }

            var isCorrect = await _userManager.CheckPasswordAsync(user, request.Password);

            if (isCorrect)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    result.IsEmailConfirmed = false;
                    result.Errors.Add("Преди да можете да се логнете Вашият email трябва да бъде потвърден.");
                    return result;
                }

                result.IsEmailConfirmed = true;

                var userRoles = await _userManager.GetRolesAsync(user);
                var (token, expiresAt) = GenerateJwtToken(user, userRoles);

                result.Token = token;
                result.ExpiresAt = expiresAt;
            }

            return result;
        }

        public async Task<AuthResult> Update(UpdateUserRequest request)
        {
            var result = new AuthResult();

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser == null)
            {
                result.Result = false;
                result.Errors = new List<string>() { "Потребител с този еmail не съществува" };
            }

            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(existingUser);

            if (!updateResult.Succeeded)
            {
                result.Result = false;
                result.Errors = updateResult.Errors.Select(x => x.Description).ToList();
                return result;
            }

            if (!string.IsNullOrEmpty(request.OldPassword) && !string.IsNullOrEmpty(request.NewPassword))
            {
                var passwordUpdateResult = await _userManager.ChangePasswordAsync(existingUser, request.OldPassword, request.NewPassword);

                if (!passwordUpdateResult.Succeeded)
                {
                    result.Result = passwordUpdateResult.Succeeded;
                    result.Errors = TranslateErrors(passwordUpdateResult.Errors);
                    return result;
                }

                var userRoles = await _userManager.GetRolesAsync(existingUser);
                var (token, expiresAt) = GenerateJwtToken(existingUser, userRoles);

                result.Token = token;
                result.ExpiresAt = expiresAt;
            }

            result.Result = true;
            return result;
        }

        public async Task<AuthResult> Delete(string email)
        {
            var result = new AuthResult();

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                result.Result = false;
                result.Errors = new List<string>() { "Потребител с този еmail не съществува" };
            }

            var userRaces = (await _getUserRacesByUserQuery.Get(existingUser!.Id)).ToList();
            userRaces.ForEach(x => _deleteUserRaceCommand.Execute(x.Id.Value));
            await _deleteUserSettingsCommand.Execute(existingUser!.Id);

            var deleteResult = await _userManager.DeleteAsync(existingUser);
            if (!deleteResult.Succeeded)
            {
                result.Result = false;
                return result;
            }

            result.Result = true;
            return result;
        }

        private async Task MakeUsAdminsAsync(User user)
        {
            if (user.Email == "dilyan.e.dimitrov@gmail.com" || user.Email == "plamen.t.manov@gmail.com")
            {
                var adminRole = await CreateAdminRoleAsync();

                if (adminRole != null)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        private async Task<IdentityRole> CreateAdminRoleAsync()
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");

            if (adminRole == null)
            {
                adminRole = new IdentityRole() { Name = "Admin" };
                var adminRoleResult = await _roleManager.CreateAsync(adminRole);

                return adminRoleResult.Succeeded ? adminRole : null;
            }

            return adminRole;
        }

        private (string, DateTime?) GenerateJwtToken(User user, IEnumerable<string> roles)
        {
            // Now its ime to define the jwt token which will be responsible of creating our tokens
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // We get our secret from the appsettings
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Value.Secret);

            var claims = new List<Claim>();
            claims.Add(new Claim("Id", user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var name = $"{user.FirstName} {user.LastName}".Trim();
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, string.IsNullOrEmpty(name) ? user.UserName : name));

            if (roles != null && roles.Any())
            {
                roles.ToList().ForEach(x => claims.Add(new Claim(ClaimTypes.Role, x)));
            }

            // we define our token descriptor
            // We need to utilise claims which are properties in our token which gives information about the token
            // which belong to the specific user who it belongs to
            // so it could contain their id, name, email the good part is that these information
            // are generated by our server and identity framework which is valid and trusted
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // the life span of the token needs to be shorter and utilise refresh token to keep the user signedin
                // but since this is a demo app we can extend it to fit our current need
                Expires = DateTime.UtcNow.AddDays(180),
                // here we are adding the encryption alogorithim information which will be used to decrypt our token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return (jwtToken, tokenDescriptor.Expires);
        }

        private List<string> TranslateErrors(IEnumerable<IdentityError> errors)
        {
            return errors.Select(e =>
            {
                switch (e.Code)
                {
                    case "PasswordMismatch": return "Грешна парола.";
                    case "PasswordTooShort": return "Паролата трябва да е поне 6 знака.";
                    case "PasswordRequiresNonAlphanumeric": return "Паролата трябва да съдържа поне 1 специален символ.";
                    case "PasswordRequiresDigit": return "Паролата трябва да съдържа поне 1 цифра.";
                    case "PasswordRequiresUpper": return "Паролата трябва да съдържа поне 1 голяма буква.";
                    default: return "";
                }
            }).ToList();
        }

        private async Task<bool> SendEmailConfirmation(User user)
        {
            var emailConfirmationToken = HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(user));
            var emailConfirmationPath = _config["EmailConfirmationPath"];
            var callbackUrl = $"{emailConfirmationPath}?token={emailConfirmationToken}&email={user.Email}";

            var body = $@"
Здравейте, {user.FirstName} {user.LastName}!
<br />
<br />
Потвърдете Вашата регистрация, като кликнета на линка:
<a target=""_blank"" href=""{callbackUrl}"" style=""border: 1px solid white; padding: 8px; height: 36px; background-color: #71ae72; font-family: Roboto, ""Helvetica Neue"", sans-serif; font-size: 14px; color: white; font-weight: 700; cursor: pointer; text-decoration:none;"">
    Потвърди регистрация
</a>
<br />
<br />
Поздрави,
<br />
Eкипът на <a href=""http://racecalendar.bg/"" target=""_blank"" >racecalendar.bg</a>
";

            var isSendSucceeded = await _mailSender.SendAsync(user.Email, "Потвърди регистрация", body);

            return isSendSucceeded;
        }
    }
}
