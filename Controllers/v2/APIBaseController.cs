using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestCase.Models.Auth;

namespace TestCase.Controllers.v2
{
    public class APIBaseController : ControllerBase
    {

        private Device _device;

        public Device Device
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return null;
                string clientId = User.Claims.FirstOrDefault(c => c.Type == "clientId")?.Value;
                string deviceId = User.Claims.FirstOrDefault(c => c.Type == "deviceId")?.Value;
                _device = new Device()
                {
                    Id = Guid.Parse(deviceId),
                    DeviceType = User.Claims.FirstOrDefault(c => c.Type == "deviceType")?.Value,
                    ClientId = Guid.Parse(clientId),
                    Name = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    ClientName = User.Claims.FirstOrDefault(c => c.Type == "clientName")?.Value
                };
                return _device;
            }
        }
    }
}
