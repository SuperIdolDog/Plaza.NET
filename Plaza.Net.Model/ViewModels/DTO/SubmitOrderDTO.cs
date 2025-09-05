using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class SubmitOrderDTO
    {
        public string? Code { get; set; }
        public int CustomerId { get; set; }
        public int StoreId { get; set; }
        public int DeliveryType { get; set; }
        public DateTime? PickupDate { get; set; }
        public int OrderStatuItemId { get; set; }
        public SubmitAddressDTO? Address { get;set; }
      
        public List<SubmitOrderItemDTO> Items { get; set; } = null!;
    }
    public class SubmitAddressDTO
    {
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string City { get; set; } = null!;
        public string County { get; set; } = null!;
        public string Detail { get; set; } = null!;
    }
    public class SubmitOrderItemDTO
    {
        public int SkuId { get;set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
