using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class OrderItemViewModel
    {
        public OrderEntity Order { get; set; } = null!;
        public IEnumerable<OrderItemEntity> OrderItems { get; set; } = null!;
        public IEnumerable<ProductEntity> Products { get; set; } = null!;
        public int StoreId { get; set; }
        
    }
}
