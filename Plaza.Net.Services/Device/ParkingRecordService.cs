using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Device;
using Plaza.Net.IServices.Device;
using Plaza.Net.Model.Entities.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Device
{
   public class ParkingRecordService : BaseServices<ParkingRecordEntity>, IParkingRecordService
    {
        public ParkingRecordService(IBaseRepository<ParkingRecordEntity> repository) : base(repository)
        {
        }
    }
}
