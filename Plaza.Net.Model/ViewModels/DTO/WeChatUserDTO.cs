using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    /// <summary>
    /// 登录成功返回的数据结构
    /// </summary>
    public class WeChatUserDTO
    {


            /// <summary>
            /// JWT
            /// </summary>
            public string Token { get; set; } = string.Empty;

            /// <summary>
            /// 有效期（秒）
            /// </summary>
            public int ExpiresIn { get; set; }

            /// <summary>
            /// 当前登录用户信息
            /// </summary>
            public UserDto User { get; set; } = new();
        }

        /// <summary>
        /// 精简用户视图
        /// </summary>
        public class UserDto
        {
            public int Id { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string? NickName { get; set; }
            public string? AvatarUrl { get; set; }
        }

    
}
