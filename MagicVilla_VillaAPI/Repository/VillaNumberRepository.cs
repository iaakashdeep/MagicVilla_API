using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApiDBContext _apiDBContext;
        public VillaNumberRepository(ApiDBContext apiDBContext):base(apiDBContext)
        {
            _apiDBContext = apiDBContext;
        }
        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
        {
            entity.UpdatedDate=DateOnly.FromDateTime(DateTime.UtcNow);
            _apiDBContext.VillaNumbers.Update(entity);
            await _apiDBContext.SaveChangesAsync();
            return entity;
        }
    }
}
