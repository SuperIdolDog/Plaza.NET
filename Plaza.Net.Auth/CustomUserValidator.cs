using Microsoft.AspNetCore.Identity;
using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Plaza.Net.Auth
{
    public class CustomUserValidator<T> : IUserValidator<UserEntity> where T : class
    {


        public Task<IdentityResult> ValidateAsync(UserManager<UserEntity> manager, UserEntity user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNameRequired",
                    Description = "用户名不能为空"
                }));
              
            }

            // 自定义验证逻辑 - 允许中文
            if (!Regex.IsMatch(user.UserName, @"^[\u4e00-\u9fa5a-zA-Z0-9_\-@\+\.]+$"))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidUserName",
                    Description = "用户名只能包含中文、英文、数字和特定符号(-_@+.)"
                }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
