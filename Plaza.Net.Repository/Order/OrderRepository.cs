using Plaza.Net.IRepository.Order;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Order
{
    internal class OrderRepository : BaseRepository<OrderEntity>, IOrderRepository
    {
        public OrderRepository(EFDbContext context) : base(context)
        {
        }
    }
}
