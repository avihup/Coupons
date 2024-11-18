using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestCase.Interfaces.Auth;
using TestCase.Interfaces.Repositories;
using TestCase.Models.Auth;
using TestCase.Models.Database;

namespace TestCase.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IClientsRepository _clientsRepository;
        private readonly IUsersRespository _usersRespository;
        private readonly JwtSettings _jwtSettings;
        public AuthService(
            IClientsRepository clientsRepository,
            IUsersRespository usersRespository,
            IOptions<JwtSettings> jwtSettings)
        {
            _clientsRepository = clientsRepository;
            _usersRespository = usersRespository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _usersRespository.GetUserNameAsync(request.UserName);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var token = GenerateJwtToken(user, user.Client);

            return new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                ClientId = user.Client?.Id,
                ClientName = user.Client?.Name
            };
        }

        private string GenerateJwtToken(UserDto user, ClientDto client)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new("userId", user.Id.ToString())
            };

            if (client != null)
            {
                claims.Add(new("clientId", client.Id.ToString()));
                claims.Add(new("clientName", client.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
