using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class ForgotPasswordDTO
    {
        [Display(Name = "邮箱")]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Display(Name = "验证码")]
        public string Code { get; set; } = null!;
        [Display(Name = "密码")]
        [Required(ErrorMessage = "密码不能为空")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Display(Name = "确认密码")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密码与确认密码不匹配。")]
        public string ConfirmPassword { get; set; } = null!;


    }
}
