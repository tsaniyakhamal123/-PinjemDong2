using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace homepageJUnpro
{
    public partial class DetailProduk : Window
    {
        private Barang _product; // Menyimpan data produk yang sedang ditampilkan
        private int _userID;

        public DetailProduk(Barang product, int userID)
        {
            InitializeComponent();

            _product = product; // Menyimpan data produk untuk digunakan di halaman checkout
            _userID = userID;
            LoadProductDetails(product); // Menampilkan detail produk ke UI
        }

        // Menampilkan detail produk
        private void LoadProductDetails(Barang product)
        {
            ProductName.Text = product.Name;
            ProductPrice.Text = $"Rp {product.Price:N0}";
            ProductStock.Text = $"Stok: {product.Stock}";

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

        // Event handler untuk tombol kembali
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(1); // Ganti "1" dengan UserId yang sesuai
            mainWindow.Show();
            this.Close();
        }

        // Event handler untuk tombol checkout
        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
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

    // Class Checkout sebagai pelengkap agar tidak ada error referensi
    public partial class Checkout : Window
    {
        private Barang _product;

        public Checkout(Barang product)
        {
            InitializeComponent();
            _product = product;
        }
    }
}
