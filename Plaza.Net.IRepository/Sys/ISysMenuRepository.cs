
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.ViewModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.IRepository.Sys
{
    public interface ISysMenuRepository : IBaseRepository<SysMenuEntity>
    {
        Task<List<SysMenuEntity>> GetAllMenusByRoleIdAsync(int roleId);
    }
}
