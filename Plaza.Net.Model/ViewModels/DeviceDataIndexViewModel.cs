using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class DeviceDataIndexViewModel
    {
        public IEnumerable<DeviceTypeEntity> DeviceTypes { get; set; } = null!;
        public IEnumerable<DeviceEntity> Devices { get; set; } = null!;
        public IEnumerable<DictionaryItemEntity> DeviceDataUnits { get; set; } = null!;
    }
}
