﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Npgsql;

namespace homepageJUnpro
{
    public partial class AdminPage : Window
    {
        private string connstring = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";
        private int _adminId;  // Store the passed adminId

        public ObservableCollection<Barang> Barangg { get; set; }

        // Constructor accepting the adminId
        public AdminPage(int adminId)
        {
            _adminId = adminId;  // Set the adminId
            InitializeComponent();
            Barangg = new ObservableCollection<Barang>();
            DataContext = this; // Bind to data context
            LoadProducts(); // Load data from database
        }

        // Load products from the database
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
                                Stock = reader.GetInt32(4)
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

        // Display products in the UI
        private void DisplayProducts(ObservableCollection<Barang> products)
        {
            ItemsPanel.Children.Clear();

            foreach (var product in products)
            {
                ItemsPanel.Children.Add(CreateProductDisplay(product));
            }
        }

        // Create a display for a product
        private Border CreateProductDisplay(Barang product)
        {
            var border = new Border
            {
                Width = 600,
                Height = 150,
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(15),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(230, 183, 185)),
                BorderThickness = new Thickness(1)
            };

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

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

            var priceTextBlock = new TextBlock
            {
                Text = $"Rp {product.Price:N0}",
                FontSize = 14,
                Foreground = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 5, 10, 5)
            };
            stackPanel.Children.Add(priceTextBlock);

            var stockTextBlock = new TextBlock
            {
                Text = $"Stok: {product.Stock}",
                FontSize = 12,
                Foreground = Brushes.Gray,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 5, 10, 5)
            };
            stackPanel.Children.Add(stockTextBlock);

            // Delete button with image
            var deleteButton = new Button
            {
                Width = 40,
                Height = 40,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(10, 0, 0, 0)
            };

            var deleteIcon = new Image
            {
                Source = new BitmapImage(new Uri("/Resource/Trash 2.png", UriKind.Relative)),
                Width = 20,
                Height = 20,
                Stretch = Stretch.Uniform
            };

            deleteButton.Content = deleteIcon;
            deleteButton.Click += (s, e) => DeleteProduct(product);

            stackPanel.Children.Add(deleteButton);

            border.Child = stackPanel;
            return border;
        }

        // Delete product from the database
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

                    Barangg.Remove(product);
                    DisplayProducts(Barangg);
                    MessageBox.Show("Product deleted successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting product: {ex.Message}");
                }
            }
        }

        // Search functionality
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text?.Trim();

            if (string.IsNullOrEmpty(query))
            {
                DisplayProducts(Barangg);
            }
            else
            {
                var filteredProducts = new ObservableCollection<Barang>(Barangg.Where(b => b.Name.Contains(query, StringComparison.OrdinalIgnoreCase)));
                DisplayProducts(filteredProducts);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox_TextChanged(sender, null);
        }
    }
}