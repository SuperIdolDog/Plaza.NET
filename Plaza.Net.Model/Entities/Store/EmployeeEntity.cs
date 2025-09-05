using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Store
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public class EmployeeEntity : BaseEntity
    {
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 性别：0-未知, 1-男, 2-女
        /// </summary>
        public int Gender { get; set; }


        /// <summary>
        /// 工资
        /// </summary>
        public decimal Wage { get; set; }

        /// <summary>
        /// 佣金率
        /// </summary>
        public double CommissionRate { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; } = null!;

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// 员工角色ID
        /// </summary>
        public int EmployeeRoleId { get; set; }

        /// <summary>
        /// 员工角色
        /// </summary>
        public virtual EmployeeRoleEntity EmployeeRole { get; set; } = null!;

        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 店铺
        /// </summary>
        public virtual StoreEntity Store { get; set; } = null!;

        /// <summary>
        /// 订单集合
        /// </summary>
        public virtual ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
    }
}
