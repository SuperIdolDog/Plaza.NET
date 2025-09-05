using Plaza.Net.Model.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Store
{
    /// <summary>
    /// 员工角色
    /// </summary>
    public class EmployeeRoleEntity : BaseEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; } = null!;
        /// <summary>
        /// 商店ID
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 商店
        /// </summary>
        public virtual StoreEntity Store { get; set; } = null!;


        /// <summary>
        /// 员工集合
        /// </summary>
        public virtual ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();
    }
}
