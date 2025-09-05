using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class DeviceIndexViewModel
    {
        public IEnumerable<DeviceTypeEntity> DeviceTypes { get; set; } = null!;
        public IEnumerable<FloorEntity> Floors { get; set; } = null!;
        public IEnumerable<PlazaEntity> Plazas { get; set; } = null!;
        public IEnumerable<DictionaryItemEntity> DeviceStatuses { get; set; } = null!;
    }
}
