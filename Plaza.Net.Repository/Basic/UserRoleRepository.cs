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
    public class UserRoleRepository : BaseRepository<UserRoleEntity>, IUserRoleRepository
    {
        private readonly EFDbContext _dbContext;
        public UserRoleRepository(EFDbContext context) : base(context)
        {
             _dbContext = context;
        }
    }
}
