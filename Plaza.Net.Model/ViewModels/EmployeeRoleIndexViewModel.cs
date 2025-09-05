using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class EmployeeRoleIndexViewModel
    {
        public IEnumerable<PlazaEntity> Plazas { get; set; } = new List<PlazaEntity>();
        public IEnumerable<FloorEntity> Floors { get; set; } = new List<FloorEntity>();
        public IEnumerable<StoreEntity> Stores { get; set; } = new List<StoreEntity>();

    }
}
