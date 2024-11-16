using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinjemDong
{
    public class Sewa_Barang
    {
        private int rent_ID;
        public DateTime rent_date { get; set; }
        public DateTime return_date { get; set; }
        private int renter_ID;
        public string alamat_pengiriman { get; set; } = string.Empty;
        public string note { get; set; } = string.Empty;
        public string status_order { get; set; } = string.Empty;
        public void add_ulasan()
        {

        }

        public void make_order()
        {

        }
    }
}
