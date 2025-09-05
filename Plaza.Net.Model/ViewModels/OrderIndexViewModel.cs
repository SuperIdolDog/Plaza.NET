using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.Entities.Sys;

namespace Plaza.Net.Model.ViewModels
{
    public class OrderIndexViewModel
    {
        public IEnumerable<DictionaryItemEntity> OrderStatusItems { get; set; } = null!;
        public IEnumerable<FloorEntity> Floors { get; set; } = null!;
        public IEnumerable<StoreEntity> Stores { get; set; } = null!;
        public IEnumerable<PlazaEntity> Plazas { get; set; } = null!;
        public IEnumerable<EmployeeEntity> Employees { get; set; } = null!;
    }
}
