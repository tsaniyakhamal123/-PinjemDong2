using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace homepageJUnpro
{
    public partial class DetailProduk : Window
    {
        private Barang _product; // Objek barang yang sedang ditampilkan
        private int _userID; // ID pengguna yang sedang login

        public DetailProduk(Barang product, int userID)
        {
            InitializeComponent(); // Inisialisasi UI
            _product = product; // Menyimpan data produk
            _userID = userID; // Menyimpan ID pengguna
            LoadProductDetails(product); // Memuat detail produk ke UI
        }

        private void LoadProductDetails(Barang product)
        {
            // Set detail produk ke UI
            ProductName.Text = product.Name; // Nama produk
            ProductPrice.Text = $"Rp {product.Price:N0}"; // Harga produk
            ProductStock.Text = $"Stok: {product.Stock}"; // Stok produk
            ProductCategory.Text = $"Kategori: {product.Category ?? "Kategori tidak tersedia"}"; // Kategori produk
            ProductDescription.Text = product.Description ?? "Deskripsi tidak tersedia."; // Deskripsi produk
            ProductAddress.Text = $"Alamat: {product.Address ?? "Alamat tidak tersedia."}"; // Alamat produk

            // Set gambar produk
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
                    ProductImage.Source = bitmap; // Menampilkan gambar di UI
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }

        // Event handler untuk tombol Kembali
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(_userID); // Membuka halaman utama
            mainWindow.Show();
            this.Close(); // Menutup halaman Detail Produk
        }

        // Event handler untuk tombol Checkout
        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkoutPage = new Checkout(_product, _userID); // Membuka halaman checkout
                checkoutPage.Show();
                this.Close(); // Menutup halaman Detail Produk
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to checkout: {ex.Message}");
            }
        }
    }
}
