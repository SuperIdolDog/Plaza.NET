using Microsoft.EntityFrameworkCore;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.IServices.Sys
{
    public interface ISysMenuService : IBaseServices<SysMenuEntity>
    {
        Task<List<SysMenuEntity>> GetAllMenusByRoleIdAsync(int roleId);

    }
}
