using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.IRepository.Sys
{
    public interface IDictionaryItemRepository:IBaseRepository<DictionaryItemEntity>
    {
        Task<IEnumerable<DictionaryItemEntity>> GetItemPagedListByAsync(
         int parentid,
         int pageIndex,
         int pageSize,
         Expression<Func<DictionaryItemEntity, bool>> predicate);
    }
}
