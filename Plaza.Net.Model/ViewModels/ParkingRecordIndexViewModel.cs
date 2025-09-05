using Plaza.Net.Model.Entities.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class ParkingRecordIndexViewModel
    {
        public IEnumerable<DeviceEntity> Devices { get; set; } = null!;
    }
}
