using Plaza.Net.IRepository;
using Plaza.Net.IServices.Order;
using Plaza.Net.Model.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Services.Order
{
   public class PaymentRecordService : BaseServices<PaymentRecordEntity>, IPaymentRecordService
    {
        public PaymentRecordService(IBaseRepository<PaymentRecordEntity> repository) : base(repository)
        {
        }
    }
}
