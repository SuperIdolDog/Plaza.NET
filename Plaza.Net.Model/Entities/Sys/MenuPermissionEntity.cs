using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Sys
{
    public class MenuPermissionEntity:BaseEntity
    {
        public int MenuId { get; set; }
        public virtual SysMenuEntity Menu { get; set; } = null!;

        public int PermissionId { get; set; }
        public virtual PermissionEntity Permission { get; set; } = null!;
    }
}
