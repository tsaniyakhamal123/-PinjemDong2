using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Npgsql;

namespace homepageJUnpro
{
    public partial class BarangPemilik : Window
    {
        private int _pemilikId;  // ID pemilik
        public ObservableCollection<Barang> Barangg { get; set; }

        public BarangPemilik(int pemilikId)
        {
            InitializeComponent();
            _pemilikId = pemilikId;
            Barangg = new ObservableCollection<Barang>();
            LoadPemilikBarang();  // Memuat barang yang ditambahkan oleh pemilik
        }

        private void LoadPemilikBarang()
        {
            try
            {
                var barangList = GetBarangByPemilikId(_pemilikId);

                // Clear the existing items (if any) and load the new products
                ItemsPanel.Children.Clear();
                Barangg.Clear(); // Make sure Barangg is cleared before reloading

                foreach (var barang in barangList)
                {
                    Barangg.Add(barang);
                    var border = CreateProductDisplay(barang);
                    ItemsPanel.Children.Add(border);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }


        private List<Barang> GetBarangByPemilikId(int pemilikId)
        {
            var barangList = new List<Barang>();

            using (var connection = new NpgsqlConnection("Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong"))
            {
                connection.Open();
                var query = "SELECT barang_id, owner_id, nama_barang, stock, ulasan, kategori, harga, deskripsi, alamat, gambar FROM barang WHERE owner_id = @pemilikId";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@pemilikId", pemilikId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var barang = new Barang
                            {
                                Id = reader.GetInt32(0),
                                OwnerId = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Stock = reader.GetInt32(3),
                                Ulasan = reader.IsDBNull(4) ? "No review available" : reader.GetString(4),
                                Category = reader.GetString(5),
                                Price = reader.GetDecimal(6),
                                Description = reader.IsDBNull(7) ? "No description available" : reader.GetString(7),
                                Address = reader.IsDBNull(8) ? "No address available" : reader.GetString(8),
                                ImagePath = reader.IsDBNull(9) ? null : Convert.ToBase64String((byte[])reader[9])
                            };
                            barangList.Add(barang);
                        }
                    }
                }
            }
            return barangList;
        }

        private Border CreateProductDisplay(Barang barang)
        {
            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(230, 183, 185)),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(15),
                Width = 300
            };

            var stackPanel = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };

            var image = new Image
            {
                Width = 150,
                Height = 150,
                Margin = new Thickness(10)
            };

            try
            {
                if (!string.IsNullOrEmpty(barang.ImagePath))
                {
                    var imageData = Convert.FromBase64String(barang.ImagePath);
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

            stackPanel.Children.Add(image);

            var nameTextBlock = new TextBlock
            {
                Text = barang.Name,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(230, 183, 185)),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(nameTextBlock);

            var priceTextBlock = new TextBlock
            {
                Text = $"Rp {barang.Price:N0}",
                FontSize = 14,
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(priceTextBlock);

            var stockTextBlock = new TextBlock
            {
                Text = $"Stok: {barang.Stock}",
                FontSize = 12,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(stockTextBlock);

            // Button Stack for Edit and Delete
            var buttonStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            // Edit Button
            var editButton = new Button
            {
                Width = 30, // Adjusted width
                Height = 30, // Adjusted height
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(5),
                Content = new Image
                {
                    Source = new BitmapImage(new Uri("/Resource/Edit.png", UriKind.Relative)),
                    Stretch = Stretch.Uniform, // Ensure the image fits inside the button without being cut off
                    Width = 20,  // Adjusted size for the image
                    Height = 20  // Adjusted size for the image
                }
            };
            editButton.Click += (s, e) => EditBarang(barang);
            buttonStackPanel.Children.Add(editButton);

            // Delete Button
            var deleteButton = new Button
            {
                Width = 30, // Adjusted width
                Height = 30, // Adjusted height
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(5),
                Content = new Image
                {
                    Source = new BitmapImage(new Uri("/Resource/Trash 2.png", UriKind.Relative)),
                    Stretch = Stretch.Uniform, // Ensure the image fits inside the button without being cut off
                    Width = 20,  // Adjusted size for the image
                    Height = 20  // Adjusted size for the image
                }
            };
            deleteButton.Click += (s, e) => DeleteBarang(barang);
            buttonStackPanel.Children.Add(deleteButton);

            stackPanel.Children.Add(buttonStackPanel);
            border.Child = stackPanel;

            return border;
        }

        private void EditBarang(Barang barang)
        {
            // Pass both Barang object and pemilikId to EditProductPage constructor
            var editPage = new EditProductPage(barang, _pemilikId); // Pass _pemilikId here
            editPage.Show();
            this.Close();
        }

        private void DeleteBarang(Barang barang)
        {
            var result = MessageBox.Show($"Are you sure you want to delete {barang.Name}?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new NpgsqlConnection("Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong"))
                    {
                        connection.Open();
                        var query = "DELETE FROM barang WHERE barang_id = @barangId";
                        var cmd = new NpgsqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@barangId", barang.Id);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Product deleted successfully.");
                    LoadPemilikBarang(); // Refresh the product list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting product: {ex.Message}");
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = SearchBox.Text.Trim().ToLower(); // Make sure to use .Trim() and .ToLower() for better comparison.

            if (string.IsNullOrEmpty(searchQuery))
            {
                // If the search box is empty, reload all products.
                LoadPemilikBarang();
            }
            else
            {
                // Filter products based on the search query
                var filteredProducts = new ObservableCollection<Barang>(
                    Barangg.Where(product => product.Name.ToLower().Contains(searchQuery)) // Ensure both sides are lower case.
                );

                // Display filtered products
                DisplayProducts(filteredProducts);
            }
        }


        private void DisplayProducts(ObservableCollection<Barang> products)
        {
            ItemsPanel.Children.Clear(); // Clear the previous list of products

            foreach (var product in products)
            {
                ItemsPanel.Children.Add(CreateProductDisplay(product)); // Create and add each product display
            }
        }


        // Back button to navigate to MainWindow
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(_pemilikId); // Pass _pemilikId to MainWindow
            mainWindow.Show();
            this.Close();
        }

    }
}
