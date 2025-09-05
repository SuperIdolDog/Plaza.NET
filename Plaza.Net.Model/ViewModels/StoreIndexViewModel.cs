using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.User;
using System.Collections.Generic;

namespace Plaza.Net.Model.ViewModels
{
    public class StoreIndexViewModel
    {
        public IEnumerable<StoreTypeEntity> StoreTypes { get; set; } = new List<StoreTypeEntity>();
        public IEnumerable<FloorEntity> Floors { get; set; } = new List<FloorEntity>();
        public IEnumerable<PlazaEntity> Plazas { get; set; } = new List<PlazaEntity>();
        public IEnumerable<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}
