using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository.Order;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Order
{
    public class OrderItemRepository : BaseRepository<OrderItemEntity>, IOrderItemRepository
    {
        private readonly EFDbContext _dbContext;
        private readonly DbSet<OrderItemEntity> _dbSet;
        public OrderItemRepository(EFDbContext context) : base(context)
        {
            _dbContext = context;
            _dbSet = context.Set<OrderItemEntity>();
        }
        //public async Task<IEnumerable<OrderItemEntity>> GetItemPagedListByAsync(
        //  int parentid,
        //  int pageIndex,
        //  int pageSize,
        //  Expression<Func<OrderItemEntity, bool>> predicate = null!,
        //    Func<IQueryable<OrderItemEntity>, IQueryable<OrderItemEntity>> include = null!)
        //{
        //    IQueryable<OrderItemEntity> query = _dbSet.Where(item => item.OrderId== parentid);

        //    if (predicate != null)
        //    {
        //        query = query.Where(predicate);
        //    }

        //    return await query
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();
        //}


  

        public async Task<decimal> GetTotalAmountAsync(int orderId)
        {
            // 使用 IQueryable 进行查询
            var query = _dbContext.Set<OrderItemEntity>()
                .Where(item => item.OrderId == orderId)
                .Select(item => item.Quantity * item.UnitPrice);

            return await query.SumAsync();
        }

        public decimal GetTotalAmount(int orderId)
        {
            // 使用同步方法计算总和
            var total = _dbContext.Set<OrderItemEntity>()
                .Where(item => item.OrderId == orderId)
                .Sum(item => item.Quantity * item.UnitPrice);

            return total;
        }
        public async Task<bool> DeleteRangeByOrderIdAsync(int orderId)
        {
            // 获取属于该订单的所有订单项
            var itemsToDelete = await _dbContext.Set<OrderItemEntity>()
                .Where(item => item.OrderId == orderId)
                .ToListAsync();

            if (itemsToDelete == null || !itemsToDelete.Any())
                return false;

            // 删除这些订单项
            _dbContext.RemoveRange(itemsToDelete);
            var result = await _dbContext.SaveChangesAsync() > 0;

            return result;
        }
    }
}
