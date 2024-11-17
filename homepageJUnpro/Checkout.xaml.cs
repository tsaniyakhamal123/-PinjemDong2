using Npgsql;
using PinjemDong;
using System;
using System.Collections.Generic;
using System.IO;
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
using Npgsql;

namespace homepageJUnpro
{
    /// <summary>
    /// Interaction logic for Checkout.xaml
    /// </summary>
    public partial class Checkout : Window
    {
        public int _userId;
        private Barang _barang;
        private int _barangId;
        DateTime _dateToday;
        OrderCheckout order1 = new OrderCheckout(1); //jumlah minimal order adalah 1
        public Checkout(Barang barang, int loggedInUserId)
        {
            InitializeComponent();
            _userId = loggedInUserId;
            _barang = barang;
            _barangId = barang.Id;
            LoadProductDetails(barang);
        }

        private int GetLoggedInUserId()
        {
            return _userId;
        }

        private NpgsqlConnection conn;
        string connString = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";
        public static NpgsqlCommand cmd;
        private string sql = null;


        // Menampilkan detail produk
        private void LoadProductDetails(Barang product)
        {
            txt_jumlah.Text = order1._jumlahBarang.ToString();
            ProductName.Text = product.Name;
            ProductPrice.Text = $"Rp {product.Price:N0}";

            // Load gambar produk
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                try
                {
                    var imageData = Convert.FromBase64String(product.ImagePath);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(imageData);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ProductImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }

        private void CekHarga(Barang product)
        {
            order1._jumlahBarang = Convert.ToInt32(txt_jumlah.Text);
            order1.inputHargaBarang(product.Price);
            order1.HitungHarga(order1._jumlahBarang, product.Price);
            nominal_subtotal.Text = $"Rp {order1.Subtotal:N0}";
            nominal_ongkir.Text = $"Rp {OrderCheckout.ongkir:N0}";
            nominal_biayaLayanan.Text = $"Rp {OrderCheckout.biayaLayanan:N0}";
            nominal_total.Text = $"Rp {order1._total:N0}";
        }

        private void checkout_Load(object sender, EventArgs e)
        {
            conn = new NpgsqlConnection(connString);
        }

        private void Count_Click(object sender, EventArgs e)
        {
            CekHarga(_barang);
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

        private void Order_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dateToday = DateTime.Now.Date;
                // Validasi input
                if (string.IsNullOrWhiteSpace(txt_namaPenerima.Text) ||
                    string.IsNullOrWhiteSpace(txt_alamat.Text) ||
                    string.IsNullOrWhiteSpace(txt_jumlah.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(comboBox_payMethod.Text) ||
                    comboBox_payMethod.Text == "Pilih Metode Pembayaran")
                {
                    MessageBox.Show("Mohon pilih metode pembayaran", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validasi angka
                if (!int.TryParse(txt_jumlah.Text, out int jumlah) || jumlah <= 0)
                {
                    MessageBox.Show("Jumlah barang yang disewa harus positif.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                using (var conn = new NpgsqlConnection(connString))
                {
                    // Simpan ke database
                    conn.Open();

                    sql = @"INSERT INTO sewa_barang (renter_id, barang_id, rent_date, nama_penerima, alamat, rent_amount, total_paid, payment_option) " +
                       "VALUES (@penyewa, @barang, @tanggal_sewa, @nama_penerima, @alamat_pengiriman, @jumlah_barang, @total_biaya, @payMethod)";
                    cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@penyewa", _userId);
                    cmd.Parameters.AddWithValue("@barang", _barangId);
                    cmd.Parameters.AddWithValue("@tanggal_sewa", _dateToday);
                    cmd.Parameters.AddWithValue("@nama_penerima", txt_namaPenerima.Text.Trim());
                    cmd.Parameters.AddWithValue("@alamat_pengiriman", txt_alamat.Text.Trim());
                    cmd.Parameters.AddWithValue("@jumlah_barang", int.Parse(txt_jumlah.Text));
                    cmd.Parameters.AddWithValue("@total_biaya", order1._total);
                    cmd.Parameters.AddWithValue("@payMethod", comboBox_payMethod.Text.Trim());

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Checkout Berhasil", "Well Done!", MessageBoxButton.OK, MessageBoxImage.Information);
                txt_namaPenerima.Text = txt_alamat.Text = null;
                History hist = new History(GetLoggedInUserId());
                hist.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "FAIL!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
