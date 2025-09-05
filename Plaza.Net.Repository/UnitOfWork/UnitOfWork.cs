using Microsoft.EntityFrameworkCore.Storage;
using Plaza.Net.IRepository.UnitOfWork;
using Plaza.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.UnitOfWork
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly EFDbContext _dbContext;
        private bool _disposed;

        public UnitOfWork(EFDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public EFDbContext GetDbClient()
        {
           return _dbContext as EFDbContext;
        }
        public void BeginTran()
        {
            
        }

        public int CommitTran()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dbContext?.Dispose();
                _disposed = true;
            }
        }

        public void RollbackTran()
        {
            var transaction=_dbContext.Database.BeginTransaction();
            try
            {
                transaction?.Rollback();
            }
            catch (Exception)
            {
                // 可以选择记录日志或进行其他处理
                throw;
            }
            finally
            {
                transaction?.Dispose();
                transaction = null;
            }
        }
    }
}
