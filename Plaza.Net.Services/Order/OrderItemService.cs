using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Order;
using Plaza.Net.IServices.Order;
using Plaza.Net.Model.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Order
{
   public class OrderItemService : BaseServices<OrderItemEntity>, IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public OrderItemService(IOrderItemRepository repository) : base(repository)
        {
            _orderItemRepository = repository;
        }

        public async Task<bool> DeleteRangeByOrderIdAsync(int id)
        {
            return await _orderItemRepository.DeleteRangeByOrderIdAsync(id);
        }

        public async Task<decimal> GetTotalAmountAsync(int id)
        {
            return await _orderItemRepository.GetTotalAmountAsync(id);
        }

        //public async Task<IEnumerable<OrderItemEntity>> GetItemPagedListByAsync(
        //    int parentid,
        //    int pageIndex,
        //    int pageSize,
        //    Expression<Func<OrderItemEntity, bool>> predicate,
        //      Func<IQueryable<OrderItemEntity>, IQueryable<OrderItemEntity>> include = null!)
        //{
        //    return await _orderItemRepository.GetItemPagedListByAsync(parentid, pageIndex, pageSize, predicate);
        //}
    }
}
