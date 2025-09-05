using Plaza.Net.IRepository.Basic;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Basic
{
    internal class StoreTypeRepository : BaseRepository<StoreTypeEntity>, IStoreTypeRepository
    {
        public StoreTypeRepository(EFDbContext context) : base(context)
        {
   
        }
    }
}
