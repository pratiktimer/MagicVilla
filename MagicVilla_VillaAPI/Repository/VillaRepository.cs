using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaRepository :Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaRepository(ApplicationDbContext db):base(db) 
        {
            _db = db; 
        }
        public async Task<Villa> UpdateAsync(Villa villa)
        {
            villa.UpdatedDate = DateTime.Now;

            _db.Villas.Update(villa);

            await SaveAsync();

            return villa;
        }
    }
}
