﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeedVechile.Domain.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpeedVechile.Infrastructure.Comman
{
    public static class ExtensionMethods
    {
        public static async Task<string>GetCurrentUserId(UserManager<IdentityUser> _userManager,IHttpContextAccessor _contextAccessor)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
                userId=user?.Id;
            }
            return userId;
        }
        public static async void SaveCommanFields(this ApplicationDbContext dbContext, UserManager<IdentityUser> _userManager, IHttpContextAccessor _contextAccessor)
        {
            var userId=await GetCurrentUserId(_userManager, _contextAccessor);

            IEnumerable<BaseModel> inserEntities=dbContext.ChangeTracker.Entries().Where(x=>x.State==EntityState.Added).Select(x=>x.Entity).OfType<BaseModel>();
            IEnumerable<BaseModel> updateEntities=dbContext.ChangeTracker.Entries().Where(x=>x.State==EntityState.Modified).Select(x=>x.Entity).OfType<BaseModel>();

            foreach (var item in inserEntities)
            {
                item.CreatedOn = DateTime.UtcNow;
                item.CreatedBy = userId;
                item.ModifiedOn = DateTime.UtcNow;
            }
            foreach (var item in updateEntities)
            {
                item.ModifiedBy = userId;
                item.ModifiedOn = DateTime.UtcNow;
            }
        }
    }
}
