using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.FromBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.IServices.Basic
{
    public interface IPlazaService : IBaseServices<PlazaEntity>
    {
        Task<List<FMPlazaNearby>> GetNearbyAsync(double lat, double lng, string city, int page = 1, int size = 20);
    }
}
