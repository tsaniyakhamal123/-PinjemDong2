using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using homepageJUnpro; // Menambahkan namespace homepageJUnpro jika Login ada di sini.
using Npgsql;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Controls.Primitives;
//using static System.Net.Mime.MediaTypeNames;
using System.Transactions;

namespace homepageJUnpro
{
    public partial class History : Window
    {
        public int _userId;
        public int _orderID;
        public Barang _barang;
        private Riwayat _riwayat;
        private string connstring = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";
        public ObservableCollection<Riwayat> OrderHistory { get; set; }

        public History(int loggedInUserId)
        {
            InitializeComponent();
            _userId = loggedInUserId;
            OrderHistory = new ObservableCollection<Riwayat>(); // ObservableCollection untuk barang
            DataContext = this; // Bind ke data context
            LoadHistories(); // Muat barang dari database
        }

        private int GetLoggedInUserId()
        {
            return _userId;
        }

        private void OpenHistoryDetail(Riwayat hist, int _userId)
        {
            var detailPage = new HistoryDetail(hist, _userId);
            detailPage.Show();
            this.Close();
        }

        private void LoadHistories()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connstring))
                {
                    connection.Open();
                    string query = "SELECT s.rent_id, b.nama_barang, s.total_paid, s.rent_amount, s.rent_date, b.gambar, s.barang_id " +
                                   "FROM sewa_barang s INNER JOIN barang b ON s.barang_id = b.barang_id";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var hist = new Riwayat
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                TotalPrice = reader.GetDecimal(2),
                                OrderAmount = reader.GetInt32(3),
                                OrderDate = reader.GetDateTime(4),
                                ImagePath = reader.IsDBNull(5) ? null : Convert.ToBase64String((byte[])reader[5]),
                                ItemId = reader.GetInt32(6),
                            };

                            OrderHistory.Add(hist);
                        }
                    }
                }

                DisplayHistories(OrderHistory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading history: {ex.Message}");
            }
        }

        private void DisplayHistories(ObservableCollection<Riwayat> Histories)
        {
            ItemsPanel.Children.Clear();

            foreach (var hist in Histories)
            {
                ItemsPanel.Children.Add(CreateProductDisplay(hist));
            }
        }

        private Border CreateProductDisplay(Riwayat hist)
        {
            var border = new Border
            {
                Width = 500,
                Height = 120,
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(10),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(230, 183, 185)), //BorderBrush = "#E6B8B7"
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand
            };

            border.MouseLeftButtonDown += (s, e) => OpenHistoryDetail(_riwayat, _userId);

            //var containerStackPanel = new StackPanel(); // Tempat semua item

            var dockPanel = new DockPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            var imageBorder = new Border
            {
                Width = 60,
                Height = 60,
                Background = new SolidColorBrush(Color.FromRgb(242, 242, 242)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 10, 0, 10)
            };

            var image = new Image
            {
                Width = 60,
                Height = 60,
                Stretch = Stretch.Uniform
            };

            try
            {
                if (!string.IsNullOrEmpty(hist.ImagePath))
                {
                    var imageData = Convert.FromBase64String(hist.ImagePath);
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
            DockPanel.SetDock(imageBorder, Dock.Left);
            dockPanel.Children.Add(imageBorder);

            var detailsStackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 5)
            };

            var nameTextBlock = new TextBlock
            {
                Text = hist.Name,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black
            };
            detailsStackPanel.Children.Add(nameTextBlock);

            var amountTextBlock = new TextBlock
            {
                Text = $"{hist.OrderAmount.ToString()} pcs",
                FontSize = 12,
                Foreground = Brushes.Gray
            };
            detailsStackPanel.Children.Add(amountTextBlock);

            var dateTextBlock = new TextBlock
            {
                Text = $"{hist.OrderDate.Date.ToString()}",
                FontSize = 12,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };
            detailsStackPanel.Children.Add(dateTextBlock);

            DockPanel.SetDock(detailsStackPanel, Dock.Left);
            dockPanel.Children.Add(detailsStackPanel);

            var rightStackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 0, 0, 0)
            };

            var priceTextBlock = new TextBlock
            {
                Text = $"Total Pesanan: Rp {hist.TotalPrice:N0}",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 0, 5, 5)

            };
            rightStackPanel.Children.Add(priceTextBlock);

            DockPanel.SetDock(rightStackPanel, Dock.Right);
            dockPanel.Children.Add(rightStackPanel);

            border.Child = dockPanel;
            return border;
        }

        /*private void OpenHistoryDetail(Riwayat hist, int userId)
        {
            var detailPage = new DetailHistory(hist, _userId);
            detailPage.Show();
            this.Close();
        }*/

        private void ChooseHist(Riwayat hist)
        {
            _riwayat = new Riwayat();
            _riwayat.Id = hist.Id;
        }
        private void OpenReviewPage(int userID, Riwayat hist)
        {
            var reviewPage = new Review(userID, hist);
            reviewPage.Show();
            this.Close();
        }
        private void RentAgain(int userID, Riwayat _riwayat)
        {
            Barang barang = new Barang();
            try
            {
                using (var connection = new NpgsqlConnection(connstring))
                {
                    int _productID = _riwayat.Id;
                    connection.Open();
                    string query = "SELECT barang_id, nama_barang, harga, gambar FROM barang";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Barang
                            {
                                Id = _productID,
                                Name = reader.GetString(0),
                                Price = reader.GetDecimal(1),
                                ImagePath = reader.IsDBNull(2) ? null : Convert.ToBase64String((byte[])reader[3])
                            };

                            if (product.Id == _productID)
                            {
                                barang = product;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }

            var CO = new Checkout(barang, userID);
            CO.Show();
            this.Hide();
        }

        // Event handler untuk HomeButton
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Pindah ke halaman MainWindow (homepage/dashboard)
            MainWindow home = new MainWindow(GetLoggedInUserId());
            home.Show();
            this.Close();
        }

        // Event handler untuk HistoryButton
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Tetap di halaman ini (opsional)
            MessageBox.Show("Anda sudah berada di halaman History.");
        }

        // Event handler untuk DashboardButton
        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Pindah ke halaman MainWindow (anggap ini juga dashboard Anda)
            MainWindow dashboard = new MainWindow(GetLoggedInUserId());
            dashboard.Show();
            this.Close();
        }

        // Event handler untuk LogOutButton
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            {
                login loginPage = new login();
                loginPage.Show();
                this.Close();
            }
        }


        // Event handler untuk SearchBox TextChanged
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Logika untuk pencarian
            var searchText = (sender as TextBox)?.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show($"Mencari: {searchText}");
            }

            string searchQuery = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchQuery))
            {
                DisplayHistories(OrderHistory);
            }
            else
            {
                var filteredProducts = new ObservableCollection<Riwayat>(
                    OrderHistory.Where(hist => hist.Name.ToLower().Contains(searchQuery)));

                DisplayHistories(filteredProducts);
            }
        }

    }
}
