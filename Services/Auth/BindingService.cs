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
    public class BindingService : IBindingService
    {
        private readonly IClientsRepository _clientsRepository;
        private readonly IKiosksRespository _kiosksRespository;
        private readonly IMachinesRepository _machinesRespository;
        private readonly JwtSettings _jwtSettings;

        public BindingService(IClientsRepository clientsRepository,
                              IKiosksRespository kiosksRespository,
                              IMachinesRepository machinesRespository,
                              IOptions<JwtSettings> jwtSettings)
        {
            _clientsRepository = clientsRepository;
            _kiosksRespository = kiosksRespository;
            _machinesRespository = machinesRespository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<BindingResponse> LoginAsync(BindingRequest request)
        {
            var kiosk = await _kiosksRespository.GetByAccessToken(request.AccessToken);
            ClientDto client;
            string token;
            if (kiosk == null)
            {
                var machine = await _machinesRespository.GetByAccessToken(request.AccessToken);
                if (machine == null)
                    throw new UnauthorizedAccessException("Invalid access token");

                client = await _clientsRepository.GetByIdAsync(machine.ClientId);

                token = GenerateJwtToken(client, machine);

                return new BindingResponse
                {
                    Token = token,
                    Name = kiosk.Name,
                    ClientId = client.Id,
                    ClientName = client.Name
                };
            }

            client = await _clientsRepository.GetByIdAsync(kiosk.ClientId);

            token = GenerateJwtToken(client, kiosk);

            return new BindingResponse
            {
                Token = token,
                Name = kiosk.Name,
                ClientId = client.Id,
                ClientName = client.Name
            };
        }

        private string GenerateJwtToken(ClientDto client, KioskDto kiosk)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, kiosk.Name),
                new("deviceId", kiosk.Id.ToString()),
                new("deviceType", "Kiosk"),
                new("clientId", client.Id.ToString()),
                new("clientName", client.Name)
            };

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

        private string GenerateJwtToken(ClientDto client, MachineDto machine)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, machine.Name),
                new("deviceId", machine.Id.ToString()),
                new("deviceType", "Machine"),
                new("clientId", client.Id.ToString()),
                new("clientName", client.Name)
            };

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
        
    }
}
