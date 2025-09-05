using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class AdminOpUserDTO
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [Display(Name = "用户名")]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [Display(Name = "电子邮箱")]
        [Required(ErrorMessage = "电子邮箱不能为空")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        /// <summary>
        /// 验证码
        /// </summary>
        [Required]
        [Display(Name = "验证码")]
        public string Code { get; set; } = null!;

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "确认密码不能为空")]
        [Display(Name = "确认密码")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密码与确认密码不匹配")]
        public string ConfirmPassword { get; set; } = null!;

        /// <summary>
        /// 手机号码
        /// </summary>
        [Phone(ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机号")]
        public string PhoneNumber { get; set; } = null!;

        /// <summary>
        /// 用户全名
        /// </summary>
        [Display(Name = "用户全名")]
        public string? FullName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        public string? IDNumber { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Display(Name = "角色ID")]
        public int UserRoleId { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        public string AvatarUrl { get; set; } = null!;
    }
}
