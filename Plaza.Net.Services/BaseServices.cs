using Plaza.Net.IRepository;
using Plaza.Net.IServices;
using Plaza.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services
{
    /// <summary>
    /// 基础服务层实现类，提供通用的业务逻辑操作
    /// </summary>
    /// <typeparam name="T">业务实体类型</typeparam>
    public class BaseServices<T> : IBaseServices<T> where T : class,IBaseEntity, new()
    {
        protected IBaseRepository<T> _repository;
        public BaseServices(IBaseRepository<T> repository)
        {
            this._repository = repository;
        }

        public async Task<bool> CreateAsync(T t)
        {
            return await _repository.CreateAsync(t);
        }

        public async Task<bool> DeleteAsync(T t)
        {
            return await _repository.DeleteAsync(t);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
           return await _repository.FindAllAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<T> GetOneByIdAsync(int id)
        {
           return await _repository.GetOneByIdAsync(id);
        }


        public async Task<bool> UpdateAsync(T t)
        {
           return await _repository.UpdateAsync(t);
        }

        public async Task<IEnumerable<T>> GetPagedListAsync(int pageIndex, int pageSize)
        {
           return await _repository.GetPagedListAsync(pageIndex, pageSize);
        }

        public async Task<int> CountAsync()
        {
            return await _repository.CountAsync();
        }

        public Task<bool> DeleteRangeAsync(int[] ids)
        {
            return _repository.DeleteRangeAsync(ids);
        }

        public async Task<IEnumerable<T>> GetManyByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.GetManyByAsync(predicate);
        }

        //public async Task<IEnumerable<T>> GetPagedListByAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null)
        //{
        //   return await _repository.GetPagedListByAsync(pageIndex, pageSize, predicate);
        //}

        public async Task<int> CountByAsync(Expression<Func<T, bool>> predicate = null!)
        {
            return await _repository.CountByAsync(predicate);
        }
        public async Task<IEnumerable<T>> GetPagedListByAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null!, Func<IQueryable<T>, IQueryable<T>> include = null!)
        {
            return await _repository.GetPagedListByAsync(pageIndex, pageSize, predicate,include);
        }

        public async Task<T> GetOneByCodeAsync(string code)
        {
            return await _repository.GetOneByCodeAsync(code);
        }

        public async Task<IEnumerable<T>> GetItemPagedListByAsync(int? parentId, int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            return await _repository.GetItemPagedListByAsync(
             parentId,
             pageIndex,
             pageSize,
             predicate,
             include);
        }
    }
}
