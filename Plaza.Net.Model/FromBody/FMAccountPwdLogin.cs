using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FromBody
{
    public class FMAccountPwdLogin
    {
        [Required(ErrorMessage = "请输入用户名或邮箱")]
        public string Account { get; set; } = null!;

        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; } = null!;

    }
}
