using Plaza.Net.IRepository;
using Plaza.Net.IServices.Device;
using Plaza.Net.Model.Entities.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Device
{
   public class DeviceService : BaseServices<DeviceEntity>, IDeviceService
    {
        public DeviceService(IBaseRepository<DeviceEntity> repository) : base(repository)
        {
        }
    }
}
