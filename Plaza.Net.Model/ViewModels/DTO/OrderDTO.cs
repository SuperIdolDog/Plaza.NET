using Plaza.Net.Model.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public int DeliveryType { get; set; }//购买方式
        public int GoodsCount { get; set; }//数量
        public int OrderStatuItemId { get; set; }
        public string? OrderStatus { get; set; } // 订单状态名称
        public string? PlazaName { get; set; }
        public string? ShippingAddress { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; } // 店铺名称
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; } // 客户名称
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; } // 员工名称
        public string Code { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }
    public class OrderItemDTO
    {
       
        public int OrderId { get; set; }
        public int SkuId { get; set; } 
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
      
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }

        public string Spec { get; set; }
    }
}
