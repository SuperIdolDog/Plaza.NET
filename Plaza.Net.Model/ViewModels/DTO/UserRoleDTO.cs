using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class UserRoleDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "用户角色标识是必填的!")]
        public string UserRoleCode { get; set; } = null!;// 用户角色标识(主键)
        [Required(ErrorMessage = "用户角色名称是必填的!")]
        [Range(1, int.MaxValue, ErrorMessage = "用户角色名称必须是一个有效的枚举值。")]
        public int UserRoleName { get; set; } // 用户角色名称（枚举类）
        [StringLength(255, ErrorMessage = "用户角色介绍的长度不能超过255个字符。")]
        public string? UserRoleDescription { get; set; }// 用户角色介绍
    }
}
