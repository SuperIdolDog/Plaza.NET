using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class UserLoginDTO
    {
        //[Display(Name = "邮箱")]
        //[Required(ErrorMessage = "邮箱不能为空")]
        //[EmailAddress]
        //public string Email { get; set; } = null!;
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "用户名不能为空")]
        public string UserName { get; set; } = null!;
        [Display(Name = "密码")]
        [Required(ErrorMessage = "密码不能为空")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Display(Name = "记住账号")]
        public bool RemeberMe { get; set; } = false;
        [Display(Name = "登录时间")]
        public DateTime LastLoginDate { get; set; }
        [Display(Name = "验证码")]
        public string? Captcha { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool LockoutOnFailure { get; set; }

        [Display(Name = "手机号")]
        [Required(ErrorMessage = "手机号不能为空")]
        public string PhoneNUmber { get; set; } = null!;

    }
}
