using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class ProductIndexViewModel
    {
        public IEnumerable<PlazaEntity> Plazas { get; set; } = null!;
        public IEnumerable<FloorEntity> Floors { get; set; } = null!;
        public IEnumerable<StoreEntity> Stores { get; set; } = null!;
        public IEnumerable<ProductTypeEntity> ProductTypes { get; set; } =null!;
    }
}
