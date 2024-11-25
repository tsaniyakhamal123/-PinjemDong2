using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Npgsql;

namespace homepageJUnpro
{
    public partial class DetailProduk : Window
    {
        private int _productId; // ID produk yang dipilih
        private int _userID;    // ID pengguna yang login
        private Barang _product; // Objek barang yang dimuat dari database

        private string connString = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";

        public DetailProduk(int productId, int userID)
        {
            InitializeComponent();

            _productId = productId;
            _userID = userID;

            try
            {
                LoadProductDetailsFromDatabase(_productId); // Muat detail produk dari database
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan saat memuat detail produk: {ex.Message}");
                Close(); // Tutup halaman jika terjadi kesalahan fatal
            }
        }

        private void LoadProductDetailsFromDatabase(int productId)
        {
            const string query = @"
                SELECT barang_id, owner_id, nama_barang, stock, ulasan, kategori, harga, deskripsi, alamat, gambar
                FROM barang
                WHERE barang_id = @barang_id;";

            using (var connection = new NpgsqlConnection(connString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@barang_id", productId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Membuat objek barang dari hasil query
                            _product = new Barang
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("barang_id")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("owner_id")),
                                Name = reader["nama_barang"]?.ToString() ?? "Nama tidak tersedia",
                                Stock = reader["stock"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("stock")) : 0,
                                Ulasan = reader["ulasan"]?.ToString() ?? "Belum ada ulasan.",
                                Category = reader["kategori"]?.ToString() ?? "Kategori tidak tersedia",
                                Price = reader["harga"] != DBNull.Value ? reader.GetDecimal(reader.GetOrdinal("harga")) : 0,
                                Description = reader["deskripsi"]?.ToString() ?? "Deskripsi tidak tersedia",
                                Address = reader["alamat"]?.ToString() ?? "Alamat tidak tersedia",
                                ImageBytes = reader["gambar"] != DBNull.Value ? (byte[])reader["gambar"] : null
                            };

                            LoadProductDetails(_product);
                        }
                        else
                        {
                            MessageBox.Show("Produk tidak ditemukan di database.");
                            Close();
                        }
                    }
                }
            }
        }

        private void LoadProductDetails(Barang product)
        {
            try
            {
                // Tampilkan data di UI
                ProductName.Text = product.Name;
                ProductPrice.Text = $"Rp {product.Price:N0}";
                ProductStock.Text = $"Stok: {product.Stock}";
                ProductCategory.Text = product.Category;
                ProductDescription.Text = product.Description;
                ProductAddress.Text = product.Address;

                if (!string.IsNullOrWhiteSpace(product.Ulasan))
                {
                    ProductDescription.Text += $"\n\nUlasan: {product.Ulasan}";
                }

                // Tampilkan gambar jika ada
                if (product.ImageBytes != null && product.ImageBytes.Length > 0)
                {
                    SetProductImage(product.ImageBytes);
                }
                else
                {
                    ProductImage.Source = null;
                    MessageBox.Show("Gambar tidak ditemukan untuk produk ini.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan saat memuat detail produk ke UI: {ex.Message}");
            }
        }

        private void SetProductImage(byte[] imageBytes)
        {
            try
            {
                var bitmap = new BitmapImage();
                using (var stream = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                ProductImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat gambar: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mainWindow = new MainWindow(_userID); // Navigasi ke halaman utama
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal kembali ke halaman utama: {ex.Message}");
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_product == null)
                {
                    MessageBox.Show("Produk tidak valid untuk checkout.");
                    return;
                }

                var checkoutPage = new Checkout(_product, _userID); // Navigasi ke halaman checkout
                checkoutPage.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal melanjutkan ke halaman checkout: {ex.Message}");
            }
        }
    }
}
