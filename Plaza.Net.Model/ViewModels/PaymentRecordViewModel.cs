using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Order;
using Plaza.Net.Model.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class PaymentRecordViewModel
    {
        //public PaymentRecordEntity PaymentRecord { get; set; } = null!;
        public IEnumerable<DictionaryItemEntity> PaymentMethods { get; set; } = null!;
        public IEnumerable<DictionaryItemEntity> PaymentStatus { get; set; } = null!;
        public IEnumerable<OrderEntity> Orders { get; set; } = null!;
        public IEnumerable<UserEntity> Users { get; set; } = null!;
    }
}
