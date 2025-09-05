
using Plaza.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.IServices
{
    /// <summary>
    /// 通用服务层接口，定义基础业务操作契约
    /// </summary>
    /// <typeparam name="T">业务实体类型</typeparam>
    public interface IBaseServices<T> where T : class,IBaseEntity,new()
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
         Task<bool> CreateAsync(T t);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
         Task<bool> DeleteAsync(T t);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> DeleteRangeAsync(int[] ids);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
         Task<bool> UpdateAsync(T t);
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
         Task<IEnumerable<T>> GetAllAsync();
        /// <summary>
        /// 根据自定义条件查询
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
         Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 根据id查询单体数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
         Task<T> GetOneByIdAsync(int id);
       
        /// <summary>
        /// 根据code查询单体数据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<T> GetOneByCodeAsync(string code);
        /// <summary>
        /// 根据自定义条件查询数据
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
         Task<IEnumerable<T>> GetManyByAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>分页结果</returns>
        Task<IEnumerable<T>> GetPagedListAsync(int pageIndex, int pageSize);
        /// <summary>
        /// 数据总数
        /// </summary>
        /// <returns></returns>
         Task<int> CountAsync();
        /// <summary>
        /// 条件查询数据总数
        /// </summary>
        /// <returns></returns>
         Task<int> CountByAsync(Expression<Func<T, bool>> predicate = null!);
        /// <summary>
        /// 条件分页查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
         Task<IEnumerable<T>> GetPagedListByAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null!, Func<IQueryable<T>, IQueryable<T>> include = null!);
        /// <summary>
        /// 子表条件分页查询
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetItemPagedListByAsync(int? parentId, int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null!, Func<IQueryable<T>, IQueryable<T>> include = null!);
    }
}
