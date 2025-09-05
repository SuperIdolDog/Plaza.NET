using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.FromBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Basic
{
    public class PlazaRepository : BaseRepository<PlazaEntity>, IPlazaRepository
    {
        private readonly EFDbContext _dbContext;

        public PlazaRepository(EFDbContext context) : base(context)
        {
            _dbContext = context;
        }


        public async Task<List<FMPlazaNearby>> GetNearbyAsync(
            double lat, double lng, string city, int page = 1, int size = 20)
        {
            const double earthRadius = 6378.137; // km

            var query = _dbContext.Plaza
                .Where(p => p.Address.Contains(city))
                .Select(p => new FMPlazaNearby
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    // ↓↓↓ 保留 2 位小数 ↓↓↓
                    Distance = Math.Round(
                        Math.Acos(
                            Math.Sin(lat * Math.PI / 180) * Math.Sin(p.Latitude * Math.PI / 180) +
                            Math.Cos(lat * Math.PI / 180) * Math.Cos(p.Latitude * Math.PI / 180) *
                            Math.Cos((lng - p.Longitude) * Math.PI / 180)) * earthRadius,
                        2)
                })
                .OrderBy(x => x.Distance)
                .Skip((page - 1) * size)
                .Take(size);

            return await query.ToListAsync();
        }
    }
}
