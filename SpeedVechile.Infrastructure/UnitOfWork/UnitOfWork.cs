using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SpeedVechile.Application.Contracts.Presistence;
using SpeedVechile.Infrastructure.Comman;
using SpeedVechile.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedVechile.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
          
        public UnitOfWork(ApplicationDbContext dbContext ,UserManager<IdentityUser> userManager,IHttpContextAccessor contextAccessor)
        {
           _dbContext = dbContext;
           _userManager = userManager;
           _contextAccessor = contextAccessor;
            Brand =new BrandRepository(_dbContext);
            VehicleType=new VehicleTypeRepository(_dbContext);
            Post=new PostRepository(_dbContext);
        }
        public IBrandRepository Brand{ get; private set; }

        public IVehicleTypeRepository VehicleType{ get; private set; }

        public IPostRepository Post{ get; private set; }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task SaveAsync()
        {
            _dbContext.SaveCommanFields(_userManager,_contextAccessor);
            await _dbContext.SaveChangesAsync();
        }
    }
}
