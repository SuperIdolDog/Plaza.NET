using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.IServices.Order
{
    public interface IOrderItemService : IBaseServices<OrderItemEntity>
    {
        Task<bool> DeleteRangeByOrderIdAsync(int id);

        //        Task<IEnumerable<OrderItemEntity>> GetItemPagedListByAsync(
        //int parentid,
        //int pageIndex,
        //int pageSize,
        //Expression<Func<OrderItemEntity, bool>> predicate,
        //  Func<IQueryable<OrderItemEntity>, IQueryable<OrderItemEntity>> include = null!);
        Task<decimal> GetTotalAmountAsync(int id);
    }
}
