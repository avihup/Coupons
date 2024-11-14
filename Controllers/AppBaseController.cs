﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestCase.Models.Auth;

namespace TestCase.Controllers
{
    public class AppBaseController : ControllerBase
    {

        private User _user;

        public User AppUser
        {
            get
            {
                if (!this.User.Identity.IsAuthenticated)
                    return null;
                string clientId = User.Claims.FirstOrDefault(c => c.Type == "clientId")?.Value;
                string userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                this._user = new User()
                {
                    UserId = Guid.Parse(userId),
                    ClientId = !string.IsNullOrEmpty(clientId) ? Guid.Parse(clientId) : null,
                    UserName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    ClientName = User.Claims.FirstOrDefault(c => c.Type == "clientName")?.Value
                };
                return this._user;
            }
        }
    }
}
