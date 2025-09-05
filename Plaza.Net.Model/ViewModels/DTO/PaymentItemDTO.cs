using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class PaymentItemDTO
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
