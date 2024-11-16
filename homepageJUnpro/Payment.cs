using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinjemDong
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string MetodePayment { get; set; } = string.Empty;
        public double TotalPembayaran { get; set; }
        public string StatusPayment { get; set; } = string.Empty;

        public void payment()
        {

        }

        public void payment_method()
        {

        }
    }
}
