using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.ViewModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Mapping
{
    public class UserRoleMapper
    {
        public static UserRoleEntity MapToUserRole(UserRegisterDTO userRegisterDTO)
        {
            // 创建一个新的 ApplicationUser 实例
            var userRole = new UserRoleEntity
            {

            };



            return userRole;
        }
    }
}
