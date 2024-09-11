using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedVechile.Application.Services.Interface
{
    public interface IUserNameService
    {
        public Task<string> GetUserName(string userId);
    }
}
