using Microsoft.AspNetCore.Identity;
using SpeedVechile.Application.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedVechile.Application.Services
{
    public class UserNameService : IUserNameService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserNameService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> GetUserName(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                return String.Empty;
            }
            var user=await _userManager.FindByNameAsync(userId);
            if (user != null)
            {
                return user.UserName;
            }
            return "NA";
        }
    }
}
