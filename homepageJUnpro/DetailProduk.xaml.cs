using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace homepageJUnpro
{
    public partial class DetailProduk : Window
    {
        private Barang _product; // Menyimpan data produk yang sedang ditampilkan
        private int _userID; // Menyimpan ID pengguna

        public DetailProduk(Barang product, int userID)
        {
            InitializeComponent();

            _product = product; // Data produk untuk checkout
            _userID = userID; // Menyimpan UserID
            LoadProductDetails(product); // Tampilkan detail produk ke UI
        }

        // Menampilkan detail produk
        private void LoadProductDetails(Barang product)
        {
            // Nama produk
            ProductName.Text = product.Name;

            // Harga produk
            ProductPrice.Text = $"Rp {product.Price:N0}";

            // Stok produk
            ProductStock.Text = $"Stok: {product.Stock}";

            // Kategori produk
            ProductCategory.Text = string.IsNullOrEmpty(product.Category)
                ? "Kategori tidak tersedia"
                : product.Category;

            // Deskripsi produk
            ProductDescription.Text = string.IsNullOrEmpty(product.Description)
                ? "Deskripsi tidak tersedia"
                : product.Description;

            // Gambar produk
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
                    MessageBox.Show($"Gagal memuat gambar: {ex.Message}");
                }
            }
            else
            {
                // Jika gambar tidak tersedia
                ProductImage.Source = null;
            }
        }

        // Event handler untuk tombol kembali ke halaman utama
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(_userID);
            mainWindow.Show();
            this.Close();
        }

        // Event handler untuk tombol checkout
        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkoutPage = new Checkout(_product, _userID);
                checkoutPage.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal navigasi ke halaman checkout: {ex.Message}");
            }
        }
    }
}
