using Plaza.Net.IRepository.Basic;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Basic
{
    public class ParkingAreaRepository : BaseRepository<ParkingAreaEntity>, IParkingAreaRepository
    {
        public ParkingAreaRepository(EFDbContext context) : base(context)
        {
        }
    }
}
