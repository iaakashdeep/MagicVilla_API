using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaRepository : Repository<VillaAPI>, IVillaRepository
    {
        private readonly ApiDBContext _dbContext;
        public VillaRepository(ApiDBContext apiDBContext):base(apiDBContext)
        {
            _dbContext = apiDBContext;
        }

        public async Task<VillaAPI> UpdateAsync(VillaAPI entity)
        {
            entity.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);
            _dbContext.VillaAPIs.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
