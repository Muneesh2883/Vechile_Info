using Microsoft.EntityFrameworkCore;
using SpeedVechile.Application.Contracts.Presistence;
using SpeedVechile.Domain.Models;
using SpeedVechile.Infrastructure.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedVechile.Infrastructure.Repositories
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            
        }
        public async Task Update(Brand brand)
        {
            var objFromDb = await _dbContext.Brand.FirstOrDefaultAsync(x => x.Id == brand.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = brand.Name;
                objFromDb.EstablishedYear = brand.EstablishedYear;

                if (brand.BrandLogo != null)
                {
                    objFromDb.BrandLogo = brand.BrandLogo;
                }
            }
            _dbContext.Update(objFromDb);
        }
    }
}
