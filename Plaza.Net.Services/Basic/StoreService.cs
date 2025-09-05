using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IServices.Basic;
using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Basic
{
   public class StoreService : BaseServices<StoreEntity>, IStoreService
    {
        public StoreService(IBaseRepository<StoreEntity> repository) : base(repository)
        {
        }
    }
}
