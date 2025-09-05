using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IServices.Basic;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.FromBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Basic
{
    public class PlazaService : BaseServices<PlazaEntity>, IPlazaService
    {
        private readonly IPlazaRepository _repository;
        public PlazaService(IPlazaRepository repository) : base(repository)
        {
            _repository = repository;
        }


        public async Task<List<FMPlazaNearby>> GetNearbyAsync(double lat, double lng, string city, int page = 1, int size = 20)
        {
            return await _repository.GetNearbyAsync(lat, lng,city, page, size);
        }
    }
}
