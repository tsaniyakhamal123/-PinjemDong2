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
    public partial class AdminPage : Window
    {
        private string connstring = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";
        public ObservableCollection<Barang> Barangg { get; set; }

        public AdminPage()
        {
            InitializeComponent();
            Barangg = new ObservableCollection
    <Barang>(); // ObservableCollection untuk barang
            DataContext = this; // Bind ke data context
            LoadProducts(); // Muat barang dari database
        }

        // Method untuk memuat data barang dari database
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

        // Method untuk menampilkan data barang di UI
        private void DisplayProducts(ObservableCollection
        <Barang> products)
        {
            ItemsPanel.Children.Clear();

            foreach (var product in products)
            {
                ItemsPanel.Children.Add(CreateProductDisplay(product));
            }
        }

        // Membuat tampilan untuk setiap barang
        private Border CreateProductDisplay(Barang product)
        {
            var border = new Border
            {
                Width = 600, // Adjusted width
                Height = 150, // Adjusted height
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(15),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(230, 183, 185)),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand
            };

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal, // Horizontal layout
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Gambar produk
            var imageBorder = new Border
            {
                Width = 100,
                Height = 100,
                Background = new SolidColorBrush(Color.FromRgb(242, 242, 242)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 10, 10, 10)
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

            // Item name
            var nameTextBlock = new TextBlock
            {
                Text = product.Name,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(230, 183, 185)),
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 5, 10, 5)
            };
            stackPanel.Children.Add(nameTextBlock);

            // Item price
            var priceTextBlock = new TextBlock
            {
                Text = $"Rp {product.Price:N0}",
                FontSize = 14,
                Foreground = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 5, 10, 5)
            };
            stackPanel.Children.Add(priceTextBlock);

            // Item stock
            var stockTextBlock = new TextBlock
            {
                Text = $"Stok: {product.Stock}",
                FontSize = 12,
                Foreground = Brushes.Gray,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 5, 10, 5)
            };
            stackPanel.Children.Add(stockTextBlock);

            // Edit Button (Image-based)
            var editButton = new Button
            {
                Width = 40,
                Height = 40,
                Margin = new Thickness(10),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Cursor = Cursors.Hand
            };

            // Create Image for Edit
            var editImage = new Image
            {
                Source = new BitmapImage(new Uri(@"C:\Users\USER\Downloads\Junpro today\homepageJUnpro\Resource\Edit.png")), // Path to your Edit icon
                Stretch = Stretch.Uniform,
                Width = 20,
                Height = 20
            };

            editButton.Content = editImage;
            editButton.Click += (s, e) => EditProduct(product);

            // Delete Button (Image-based)
            var deleteButton = new Button
            {
                Width = 40,
                Height = 40,
                Margin = new Thickness(10),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Cursor = Cursors.Hand
            };

            // Create Image for Delete
            var deleteImage = new Image
            {
                Source = new BitmapImage(new Uri(@"C:\Users\USER\Downloads\Junpro today\homepageJUnpro\Resource\Trash 2.png")), // Path to your Delete icon
                Stretch = Stretch.Uniform,
                Width = 20,
                Height = 20
            };

            deleteButton.Content = deleteImage;
            deleteButton.Click += (s, e) => DeleteProduct(product);

            // Add the image buttons to the stack panel
            stackPanel.Children.Add(editButton);
            stackPanel.Children.Add(deleteButton);

            // Attach the stack panel to the border
            border.Child = stackPanel;
            return border;
        }

        // Membuka halaman detail barang
        private void EditProduct(Barang product)
        {
            var EditProductPage = new EditProductPage();
            EditProductPage.Show();
            this.Close();
        }

        // Fungsi Delete Product
        private void DeleteProduct(Barang product)
        {
            var result = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(connstring))
                    {
                        connection.Open();
                        string query = "DELETE FROM barang WHERE barang_id = @Id";
                        using (var cmd = new NpgsqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Id", product.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Hapus produk dari ObservableCollection dan tampilkan ulang produk
                    Barangg.Remove(product);
                    MessageBox.Show("Product deleted successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting product: {ex.Message}");
                }
            }
        }

        // Event untuk tombol Log Out
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var loginPage = new login();
            loginPage.Show();
            this.Close();
        }
    }
}
