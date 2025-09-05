using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository;
using Plaza.Net.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository
{
    /// <summary>
    /// 泛型仓储基类，实现基础数据访问操作
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class,IBaseEntity, new()
    {
        private readonly EFDbContext _DbContext;
        private readonly DbSet<T> _dbSet;
        public BaseRepository(EFDbContext context)
        {
           _DbContext = context;
            _dbSet = context.Set<T>();
        }

        public async Task<bool> CreateAsync(T t)
        {
            await _dbSet.AddAsync(t);
            return await _DbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(T t)
        {
             _dbSet.Remove(t);
            return await _DbContext.SaveChangesAsync() > 0;

        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetOneByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }



        public virtual async Task<bool> UpdateAsync(T t)
        {
            // 获取实体Entry
            var entry = _DbContext.Entry(t);
            // 标记为Modified状态
            entry.State = EntityState.Modified;
            // 排除创建时间字段的更新
           
                entry.Property("CreateTime").IsModified = false;
           
            return await _DbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<T>> GetPagedListAsync(int pageIndex, int pageSize)
        {
            return await _dbSet
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<bool> DeleteRangeAsync(int[] ids)
        {
            var list= await _dbSet
                    .Where(t => ids
                    .Contains(EF.Property<int>(t, "Id")))
                    .ToListAsync();
            _dbSet.RemoveRange(list);
            return await _DbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<T>> GetManyByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPagedListByAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null!)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByAsync(Expression<Func<T, bool>> predicate = null!)
        {
            return await _dbSet.Where(predicate).CountAsync();
        }

        public async Task<IEnumerable<T>> GetPagedListByAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null!, Func<IQueryable<T>, IQueryable<T>> include = null!)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            if (include != null)
            {
                query = include(query);
            }

            return await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<T> GetOneByCodeAsync(string code)
        {
            if (code == null)
            {
                return null; // 或者根据需求抛出异常
            }

            return await _dbSet.FirstOrDefaultAsync(t => t.Code == code);
        }

        public async Task<IEnumerable<T>> GetItemPagedListByAsync(int? parentId, int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _dbSet;

            // 如果T有ParentId属性，并且parentId有值，则添加过滤条件
            var property = typeof(T).GetProperty("ParentId");
            if (property != null && parentId.HasValue)
            {
                query = query.Where(x => EF.Property<int>(x, "ParentId") == parentId.Value);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            return await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

       
    }
}
