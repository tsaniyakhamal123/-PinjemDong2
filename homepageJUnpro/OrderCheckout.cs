using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinjemDong
{
    public class OrderCheckout
    {
        private int _barangID;
        public string _namaPenerima;
        public string _alamat;
        public string _payMethod;
        public const int biayaLayanan = 5000;
        public const int ongkir = 10000;
        public int _jumlahBarang;
        private int _hargaBarang;
        public int _subtotal;
        public int _total;

        public int Subtotal { get
            {
                return _subtotal;
            }

            set
            {
                _subtotal = _jumlahBarang * _hargaBarang;
            }
        }

        public OrderCheckout(int jumlahBarang)
        {
            _jumlahBarang = jumlahBarang;
        }

        public void HitungHarga()
        {
            _total = 0;
            _total = Subtotal + ongkir + biayaLayanan;
        }
    }
}
