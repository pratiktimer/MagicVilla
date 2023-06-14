using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository
{

    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaNumberRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<VillaNumber> UpdateAsync(VillaNumber villa)
        {
            villa.UpdatedDate = DateTime.Now;

            _db.VillaNumbers.Update(villa);

            await SaveAsync();

            return villa;
        }
    }
}
