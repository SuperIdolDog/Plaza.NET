
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository.Sys;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Sys
{
    public class DictionaryItemRepository : BaseRepository<DictionaryItemEntity>, IDictionaryItemRepository
    {
        private readonly EFDbContext _DbContext;
        private readonly DbSet<DictionaryItemEntity> _dbSet;
        public DictionaryItemRepository(EFDbContext context) : base(context)
        {
            _DbContext = context;
            _dbSet = context.Set<DictionaryItemEntity>();
        }
        public async Task<IEnumerable<DictionaryItemEntity>> GetItemPagedListByAsync(
            int parentid,
            int pageIndex,
            int pageSize,
            Expression<Func<DictionaryItemEntity, bool>> predicate = null!)
        {
            IQueryable<DictionaryItemEntity> query = _dbSet.Where(item=>item.DictionaryId==parentid);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
