using Plaza.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.IRepository.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        EFDbContext GetDbClient();

        void BeginTran();

        int CommitTran();
        void RollbackTran();
    }
}
