using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.Pay
{
    public class AlipayOptions
    {
        public string AppId { get; set; } = null!;
        public string PrivateKey { get; set; } = null!;
        public string AlipayPublicKey { get; set; } = null!;
        public string GatewayUrl { get; set; } = null!;
        public string SignType { get; set; } = "RSA2";
        public string Charset { get; set; } = "UTF-8";
        public string NotifyUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
    }
}
