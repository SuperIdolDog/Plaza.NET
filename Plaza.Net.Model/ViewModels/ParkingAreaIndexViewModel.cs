using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.ViewModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class ParkingAreaIndexViewModel
    {
        public IEnumerable<DictionaryItemEntity> Parkingareas { get; set; }
        public IEnumerable<FloorEntity> Floors { get; set; }
        public IEnumerable<PlazaEntity> Plazas { get; set; }
    }
}
