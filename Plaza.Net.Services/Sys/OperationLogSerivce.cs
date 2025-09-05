using Plaza.Net.IRepository;
using Plaza.Net.IServices.Sys;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Sys
{
   public class OperationLogSerivce : BaseServices<OperationLogEntity>, IOperationLogService
    {
        public OperationLogSerivce(IBaseRepository<OperationLogEntity> repository) : base(repository)
        {
        }
    }
}
