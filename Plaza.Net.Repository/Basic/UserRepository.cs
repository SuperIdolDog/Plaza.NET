using Microsoft.EntityFrameworkCore;
using Plaza.Net.IRepository.Basic;
using Plaza.Net.IRepository.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Repository.User
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        private readonly EFDbContext _dbContext;
        public UserRepository(EFDbContext context) : base(context)
        {
            _dbContext = context;
        }
        public override async Task<bool> UpdateAsync(UserEntity user)
        {
            // 获取实体Entry
            var entry = _dbContext.Entry(user);

            // 标记为Modified状态
            entry.State = EntityState.Modified;

            // 排除创建时间字段的更新（假设字段名为 RegisterDateTime）
            if (entry.Property("RegisterDate") != null)
            {
                entry.Property("RegisterDate").IsModified = false;
            }

            // 保存更改
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
