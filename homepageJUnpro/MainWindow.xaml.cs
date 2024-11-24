using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Npgsql;

namespace homepageJUnpro
{
    public partial class MainWindow : Window
    {
        private int _userId;
        private string connstring = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";
        public ObservableCollection<Barang> Barangg { get; set; }

        public MainWindow(int loggedInUserId)
        {
            InitializeComponent();
            _userId = loggedInUserId; // Menyimpan UserId pengguna yang login
            Barangg = new ObservableCollection<Barang>(); // ObservableCollection untuk barang
            DataContext = this; // Bind ke data context
            LoadProducts(); // Muat barang dari database
        }

        private void LoadProducts()
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

                            Barangg.Add(product);
                        }
                    }
                }

                DisplayProducts(Barangg);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }

        private void DisplayProducts(ObservableCollection<Barang> products)
        {
            ItemsPanel.Children.Clear();

            foreach (var product in products)
            {
                ItemsPanel.Children.Add(CreateProductDisplay(product));
            }
        }

        private Border CreateProductDisplay(Barang product)
        {
            var border = new Border
            {
                Width = 150,
                Height = 220,
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(15),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(230, 183, 185)),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand
            };

            border.MouseLeftButtonDown += (s, e) => OpenDetailPage(product, _userId);

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
                    var imageData = Convert.FromBase64String(product.ImagePath);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new System.IO.MemoryStream(imageData);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    image.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
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

            var stockTextBlock = new TextBlock
            {
                Text = $"Stok: {product.Stock}",
                FontSize = 12,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            };
            stackPanel.Children.Add(stockTextBlock);

            border.Child = stackPanel;
            return border;
        }

        private void OpenDetailPage(Barang product, int _userId)
        {
            var detailPage = new DetailProduk(product, _userId); // Passing product and user ID
            detailPage.Show();
            this.Close();
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TambahBarang addBarangPage = new TambahBarang(_userId);
            addBarangPage.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            login loginPage = new login();
            loginPage.Show();
            this.Close();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ProdukSayaButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to BarangPemilik page
            var barangPemilikPage = new BarangPemilik(_userId);
            barangPemilikPage.Show();
            this.Close();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchQuery))
            {
                DisplayProducts(Barangg);
            }
            else
            {
                var filteredProducts = new ObservableCollection<Barang>(
                    Barangg.Where(product => product.Name.ToLower().Contains(searchQuery)));

                DisplayProducts(filteredProducts);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Account editAkun = new Account(_userId);
            editAkun.Show();
            this.Close();
        }
       
    }
}
