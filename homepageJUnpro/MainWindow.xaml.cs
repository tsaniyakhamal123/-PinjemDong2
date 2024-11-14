using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Npgsql;

namespace homepageJUnpro
{
    public partial class MainWindow : Window
    {
        private int _userId;
        private const string ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=ininiya123;Database=JunproBener";
        public ObservableCollection<Barang> Barangg { get; set; }

        // Konstruktor menerima UserId setelah login
        public MainWindow(int loggedInUserId)
        {
            InitializeComponent();
            _userId = loggedInUserId; // Menyimpan UserId pengguna yang login
            Barangg = new ObservableCollection<Barang>(); // ObservableCollection untuk menampung barang
            DataContext = this; // Bind ke data context
            LoadProducts(); // Memuat produk untuk ditampilkan
        }

        // Method untuk memuat semua produk
        private void LoadProducts()
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT barang_id, nama_barang, harga, gambar FROM barang";  // Ambil semua produk

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
                                ImagePath = reader.IsDBNull(3) ? null : Convert.ToBase64String((byte[])reader[3])
                            };

                            Barangg.Add(product);  // Menambahkan produk ke ObservableCollection
                        }
                    }
                }

                DisplayProducts(Barangg);  // Tampilkan produk yang telah dimuat
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");  // Menampilkan pesan error jika gagal
            }
        }

        // Method untuk menampilkan produk di UI
        private void DisplayProducts(ObservableCollection<Barang> products)
        {
            ItemsPanel.Children.Clear(); // Bersihkan UI lama

            foreach (var product in products)
            {
                ItemsPanel.Children.Add(CreateProductDisplay(product));  // Tambahkan produk ke panel
            }
        }

        // Method untuk membuat tampilan produk (UI untuk setiap produk)
        private Border CreateProductDisplay(Barang product)
        {
            var border = new Border
            {
                Width = 150,
                Height = 200,
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(15),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(230, 183, 185)),
                BorderThickness = new Thickness(1)
            };

            var stackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var imageBorder = new Border
            {
                Width = 100,
                Height = 100,
                Background = new SolidColorBrush(Color.FromRgb(242, 242, 242)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 10, 0, 10)
            };

            var image = new Image
            {
                Width = 100,
                Height = 100,
                Stretch = Stretch.Uniform
            };

            try
            {
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    // Decode Base64 to byte array, then create BitmapImage from it
                    var imageData = Convert.FromBase64String(product.ImagePath);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(imageData);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    image.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");  // Jika ada error dalam memuat gambar
            }

            imageBorder.Child = image;
            stackPanel.Children.Add(imageBorder);

            var nameTextBlock = new TextBlock
            {
                Text = product.Name,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(230, 183, 185)),
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            };
            stackPanel.Children.Add(nameTextBlock);

            var priceTextBlock = new TextBlock
            {
                Text = $"Rp {product.Price:N0}",
                FontSize = 14,
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            };
            stackPanel.Children.Add(priceTextBlock);

            border.Child = stackPanel;
            return border;
        }

        // Pencarian produk berdasarkan nama
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        // Event handler untuk text box pencarian
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchQuery))
            {
                DisplayProducts(Barangg); // Tampilkan semua produk jika tidak ada pencarian
            }
            else
            {
                var filteredProducts = new ObservableCollection<Barang>(
                    Barangg.Where(product => product.Name.ToLower().Contains(searchQuery)));  // Filter produk berdasarkan nama

                DisplayProducts(filteredProducts);  // Tampilkan produk yang sudah difilter
            }
        }

        // Button untuk logout dan kembali ke halaman login
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            login Login = new login();  // Buka halaman login
            Login.Show();
            this.Close();  // Tutup jendela utama
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TambahBarang addbarang = new TambahBarang(_userId);
            addbarang.Show();
            this.Close();

        }
    }
}
