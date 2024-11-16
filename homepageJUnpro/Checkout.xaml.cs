using Npgsql;
using PinjemDong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace homepageJUnpro
{
    /// <summary>
    /// Interaction logic for Checkout.xaml
    /// </summary>
    public partial class Checkout : Window
    {
        public int _userId;
        private int _barangId;
        OrderCheckout order1 = new OrderCheckout(1); //jumlah minimal order adalah 1
        public Checkout(int loggedInUserId, int barangID)
        {
            InitializeComponent();
            _userId = loggedInUserId;
            _barangId = barangID;
            txt_jumlah.Text = order1._jumlahBarang.ToString();
        }

        private int GetLoggedInUserId()
        {
            return _userId;
        }

        private NpgsqlConnection conn;
        string connString = "Host=localhost;Username=postgres;Password=nasywa;Database=PinjemDong";
        public static NpgsqlCommand cmd;
        private string sql = null;

        private void CekHarga()
        {
            order1.HitungHarga();
            nominal_subtotal.Text = "Rp." + order1.Subtotal.ToString();
            nominal_ongkir.Text = "Rp." + OrderCheckout.ongkir.ToString();
            nominal_biayaLayanan.Text = "Rp." + OrderCheckout.biayaLayanan.ToString();
            nominal_total.Text = "Rp." + order1._total.ToString();
        }

        private void checkout_Load(object sender, EventArgs e)
        {
            conn = new NpgsqlConnection(connString);
        }

        private void Order_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from st_checkout(:_nama_penerima,:_alamat_pengiriman,:_jumlah_barang,:_variasi,:_subtotal,:_ongkir,:_biaya_layanan,:_total_biaya,:_payMethod)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_nama_penerima", txt_namaPenerima.Text);
                cmd.Parameters.AddWithValue("_alamat_pengiriman", txt_alamat.Text);
                cmd.Parameters.AddWithValue("_jumlah_barang", int.Parse(txt_jumlah.Text));
                cmd.Parameters.AddWithValue("_variasi", lbl_variasi.Text);
                cmd.Parameters.AddWithValue("_subtotal", order1.Subtotal.ToString());
                cmd.Parameters.AddWithValue("_ongkir", OrderCheckout.ongkir.ToString());
                cmd.Parameters.AddWithValue("_biaya_layanan", OrderCheckout.biayaLayanan.ToString());
                cmd.Parameters.AddWithValue("_total_biaya", order1._total.ToString());
                cmd.Parameters.AddWithValue("_payMethod", comboBox_payMethod.Items);
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    MessageBox.Show("Checkout Berhasil", "Well Done!", MessageBoxButton.OK, MessageBoxImage.Information);
                    conn.Close();
                    txt_namaPenerima.Text = txt_alamat.Text = null;
                    History hist = new History(GetLoggedInUserId());
                    hist.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "FAIL!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Count_Click(object sender, EventArgs e)
        {
            CekHarga();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow home = new MainWindow(_userId);
            home.Show();
            this.Close();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            History hist = new History(GetLoggedInUserId());
            hist.Show();
            this.Hide();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            TambahBarang dashboard = new TambahBarang(GetLoggedInUserId());
            dashboard.Show();
            this.Hide();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            login logout = new login();
            logout.Show();
            this.Close();
        }
    }
}
