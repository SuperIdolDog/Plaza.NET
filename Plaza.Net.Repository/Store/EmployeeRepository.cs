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
    internal class EmployeeRepository : BaseRepository<EmployeeEntity>, IEmployeeRepository
    {
        public EmployeeRepository(EFDbContext context) : base(context)
        {
        }
    }
}
