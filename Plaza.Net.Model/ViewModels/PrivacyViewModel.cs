using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels
{
    public class PrivacyViewModel
    {
        public int PlazaCount { get; set; }
        public int UserCount { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalOrderMoney { get; set; }
    }
}
