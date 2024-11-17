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
        public const decimal biayaLayanan = 5000;
        public const decimal ongkir = 10000;
        public int _jumlahBarang;
        private decimal _hargaBarang;
        public decimal _subtotal;
        public decimal _total;

        public decimal Subtotal { get
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

        public void inputHargaBarang(decimal price) 
        {
            _hargaBarang = price;
        }

        public void HitungHarga(int jumlahBarang, decimal price)
        {
            decimal checkoutAmount = Convert.ToDecimal(jumlahBarang);
            _hargaBarang = price;
            _subtotal = checkoutAmount * _hargaBarang;
            _total = 0;
            _total = _subtotal + ongkir + biayaLayanan;
        }
    }
}
