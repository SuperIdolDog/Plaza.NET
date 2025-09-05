using Plaza.Net.IRepository.Store;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.Store
{
    internal class EmployeeRoleRepository : BaseRepository<EmployeeRoleEntity>, IEmployeeRoleRepository
    {
        public EmployeeRoleRepository(EFDbContext context) : base(context)
        {
        }
    }
}
