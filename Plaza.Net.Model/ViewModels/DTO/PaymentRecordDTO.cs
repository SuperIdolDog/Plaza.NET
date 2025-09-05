using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class PaymentRecordDTO
    {
        public int Id { get; set; }
        public int PaymentMethodItemId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
        public int PaystatuItemId { get; set; }
        public string TransactionId { get; set; } = null!;
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public IEnumerable<PaymentItemDTO> Items { get; set; } = new List<PaymentItemDTO>();
    }
}
