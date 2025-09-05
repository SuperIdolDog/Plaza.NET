using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.FromBody
{
    public class FMSendSms
    {
        /// <summary>操作类型（login/register/reset...）</summary>
        public string code { get; set; } = null!;

        /// <summary>手机号</summary>
        public string mobile { get; set; } = null!;
    }
}
