using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Sys;
using Plaza.Net.IServices.Sys;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Sys
{
   public class DictionaryItemService : BaseServices<DictionaryItemEntity>, IDictionaryItemService
    {
        private readonly IDictionaryItemRepository _itemRepository;

        public DictionaryItemService(IDictionaryItemRepository repository) : base(repository)
        {
            _itemRepository = repository;
        }

        public async Task<IEnumerable<DictionaryItemEntity>> GetItemPagedListByAsync(
            int parentid,
            int pageIndex,
            int pageSize,
            Expression<Func<DictionaryItemEntity, bool>> predicate = null!)
        {
            return await _itemRepository.GetItemPagedListByAsync(parentid, pageIndex, pageSize, predicate);
        }
    }
}
