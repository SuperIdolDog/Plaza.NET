using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class StoreDto
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public int StoreTypeId { get; set; }
        public string StoreTypeName { get; set; }
        public int FloorId { get; set; }
        public string FloorName { get; set; }
        public int PlazaId { get; set; }
        public string PlazaName { get; set; }

        public string Description { get; set; }
    }
}
