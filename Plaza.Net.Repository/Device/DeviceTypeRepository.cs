using Plaza.Net.IRepository.Device;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Device
{
    internal class DeviceTypeRepository : BaseRepository<DeviceTypeEntity>, IDeviceTypeRepository
    {
        public DeviceTypeRepository(EFDbContext context) : base(context)
        {
        }
    }
}
