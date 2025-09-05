using Plaza.Net.IRepository;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IServices.Sys;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Sys
{
   public class PermissionService : BaseServices<PermissionEntity>, IPermissionService
    {

        public PermissionService(IBaseRepository<PermissionEntity> repository) : base(repository)
        {
        }


    }
}
