using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.ViewModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Mapping
{
   public class UserMapper
    {
        public static UserEntity MapToUser(UserRegisterDTO userRegisterDTO)
        {
            // 创建一个新的 ApplicationUser 实例
            var user = new UserEntity
            {
                UserName = userRegisterDTO.UserName,
                Email = userRegisterDTO.Email,
                PhoneNumber = userRegisterDTO.PhoneNumber
            };

          

            return user;
        }
    }
}
