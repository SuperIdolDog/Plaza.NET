using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class UserIndexViewModel
    {
        public IEnumerable<UserRoleEntity> Roles { get; set; } = null!;
    }
}
