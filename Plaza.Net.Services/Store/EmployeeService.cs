using Plaza.Net.IRepository;
using Plaza.Net.IServices.Store;
using Plaza.Net.Model.Entities.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Store
{
   public class EmployeeService : BaseServices<EmployeeEntity>, IEmployeeService
    {
        public EmployeeService(IBaseRepository<EmployeeEntity> repository) : base(repository)
        {
        }
    }
}
