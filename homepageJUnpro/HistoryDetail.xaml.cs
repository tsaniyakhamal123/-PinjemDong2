using Npgsql;
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

namespace homepageJUnpro
{
    /// <summary>
    /// Interaction logic for HistoryDetail.xaml
    /// </summary>
    public partial class HistoryDetail : Window
    {
        private Barang _product;
        private Riwayat _history;
        private int _userID;
        private string connstring = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";
        public HistoryDetail(Riwayat hist, int userID)
        {
            InitializeComponent();
            _history = hist; // Menyimpan data produk untuk digunakan di halaman checkout
            _userID = userID;
            _product.Id = _history.ItemId;
            LoadHistoryDetails(hist); // Menampilkan detail produk ke UI
            GetProductData(_history.ItemId);
        }

        private void GetProductData(int itemId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connstring))
                {
                    connection.Open();
                    string query = "SELECT barang_id, nama_barang, harga, gambar, stock FROM barang";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Barang
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                ImagePath = reader.IsDBNull(3) ? null : Convert.ToBase64String((byte[])reader[3]),
                                Stock = reader.GetInt32(4) // Stok produk
                            };
                            if (product.Id == _product.Id)
                            _product = product;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }

        private void LoadHistoryDetails(Riwayat hist)
        {

            ProductName.Text = _history.Name;
            ProductPrice.Text = $"Rp {_history.TotalPrice:N0}";
            ProductAmount.Text = $"Stok: {_history.OrderAmount}";

            // Load gambar produk
            if (!string.IsNullOrEmpty(_history.ImagePath))
            {
                try
                {
                    var imageData = Convert.FromBase64String(_history.ImagePath);
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

        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Buka halaman checkout dengan mengirimkan data produk
                var reviewPage = new Review(_userID, _history);
                reviewPage.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to review: {ex.Message}");
            }
        }

        private void RerentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Buka halaman checkout dengan mengirimkan data produk
                var checkoutPage = new Checkout(_product, _userID);
                checkoutPage.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to checkout: {ex.Message}");
            }
        }
    }
}
