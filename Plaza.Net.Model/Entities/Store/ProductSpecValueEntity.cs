using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.Entities.Store
{
    public class ProductSpecValueEntity:BaseEntity
    {
        public int SpecId {  get; set; }
        public virtual ProductSpecEntity Spec { get; set; } = null!;

        /// <summary>
        /// 具体规格值（如：红色、XL、256G）
        /// </summary>
        public string Value { get; set; } = null!;

 

    }
}
