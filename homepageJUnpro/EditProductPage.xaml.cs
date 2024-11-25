using System;
using System.Windows;
using Npgsql;
using System.IO;
using System.Windows.Media.Imaging;

namespace homepageJUnpro
{
    public partial class EditProductPage : Window
    {
        private Barang _barang;  // Store the passed Barang object
        private int _pemilikId;  // Store the pemilikId for back navigation

        // Constructor accepting a Barang object and pemilikId
        public EditProductPage(Barang barang, int pemilikId)
        {
            InitializeComponent();
            _barang = barang;  // Store the passed Barang object
            _pemilikId = pemilikId;  // Store the pemilikId
            PopulateFields(barang); // Populate fields with the Barang details
        }



        // Method to populate fields with Barang details
        private void PopulateFields(Barang barang)
        {
            ProductNameTextBox.Text = barang.Name;
            ProductStockTextBox.Text = barang.Stock.ToString();
            ProductPriceTextBox.Text = barang.Price.ToString("N0");
            ProductCategoryTextBox.Text = barang.Category;
            ProductDescriptionTextBox.Text = barang.Description;
            ProductImageTextBox.Text = barang.ImagePath; // Assuming it's a Base64 string
        }

        // Save button click handler to update product in the database
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update product in the database
                using (var connection = new NpgsqlConnection("Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong"))
                {
                    connection.Open();
                    string query = @"
                        UPDATE barang
                        SET nama_barang = @name,
                            stock = @stock,
                            harga = @price,
                            kategori = @category,
                            deskripsi = @description,
                            gambar = @image
                        WHERE barang_id = @id";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", _barang.Id);
                        cmd.Parameters.AddWithValue("@name", ProductNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@stock", int.Parse(ProductStockTextBox.Text));
                        cmd.Parameters.AddWithValue("@price", decimal.Parse(ProductPriceTextBox.Text));
                        cmd.Parameters.AddWithValue("@category", ProductCategoryTextBox.Text);
                        cmd.Parameters.AddWithValue("@description", ProductDescriptionTextBox.Text);

                        // Check if image is Base64 string or URL and set it accordingly
                        byte[] imageBytes = null;
                        if (!string.IsNullOrEmpty(ProductImageTextBox.Text))
                        {
                            // Assuming it's a Base64 string, convert to byte array
                            imageBytes = Convert.FromBase64String(ProductImageTextBox.Text);
                        }
                        cmd.Parameters.AddWithValue("@image", imageBytes ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating product: {ex.Message}");
            }
        }

        // Back button to navigate to the previous window (BarangPemilik)
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var barangPemilikPage = new BarangPemilik(_pemilikId); // Using the pemilikId to navigate back
            barangPemilikPage.Show();
            this.Close();
        }
    }
}
