using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestCase.Models.Auth;

namespace TestCase.Controllers.v1
{
    public class AppBaseController : ControllerBase
    {

        private AppUser _user;

        public AppUser AppUser
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return null;
                string clientId = User.Claims.FirstOrDefault(c => c.Type == "clientId")?.Value;
                string userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                _user = new AppUser()
                {
                    UserId = Guid.Parse(userId),
                    ClientId = !string.IsNullOrEmpty(clientId) ? Guid.Parse(clientId) : null,
                    UserName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    ClientName = User.Claims.FirstOrDefault(c => c.Type == "clientName")?.Value
                };
                return _user;
            }
        }
    }
}
