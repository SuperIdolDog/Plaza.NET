using Plaza.Net.IRepository.Sys;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Sys
{
    internal class PermissionRepository : BaseRepository<PermissionEntity>, IPermissionRepository
    {
        public PermissionRepository(EFDbContext context) : base(context)
        {
        }
    }
}
